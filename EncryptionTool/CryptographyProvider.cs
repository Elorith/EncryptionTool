using System.IO;
using System.Security.Cryptography;
using System.Text;

public class CryptographyProvider
{
     public const int EncryptionKeySize = 256;
     public const int EncryptionBlockSize = 128;
     private const ulong encryptionBufferSize = 1048576;
     
     #region Public API Functions
     
     public void EncryptFileWithPersonalKey(string path, string personalKey)
     {
          using (FileStream input = new FileStream(path, FileMode.Open))
          {
               string outputPath = Path.Combine(path, ".aes");
               using (FileStream output = new FileStream(outputPath, FileMode.Create))
               {
                    this.EncryptToStreamWithPersonalKey(input, output, personalKey);
               }
          }
          
          Logger.Singleton.WriteLine("'" + path + "' has been successfully encrypted.");
     }

     public void DecryptFileWithPersonalKey(string path, string personalKey)
     {
          using (FileStream input = new FileStream(path, FileMode.Open))
          {
               string outputPath = Path.Combine(Path.GetDirectoryName(path), Path.GetFileNameWithoutExtension(path));
               using (FileStream output = new FileStream(outputPath, FileMode.Create))
               {
                    this.DecryptToStreamWithPersonalKey(input, output, personalKey);
               }
          }

          Logger.Singleton.WriteLine("'" + path + "' has been successfully decrypted.");
     }

     public string EncryptStringWithPersonalKey(string original, string personalKey)
     {
          byte[] encrypted = EncryptBufferWithPersonalKey(Encoding.UTF8.GetBytes(original), personalKey);

          return Encoding.ASCII.GetString(encrypted);
     }

     public string DecryptStringWithPersonalKey(string encrypted, string personalKey)
     {
          byte[] original = EncryptBufferWithPersonalKey(Encoding.UTF8.GetBytes(encrypted), personalKey);

          return Encoding.ASCII.GetString(original);
     }

     #endregion

     #region Internal AES Functions

     private byte[] EncryptBufferWithPersonalKey(byte[] original, string personalKey)
     {
          byte[] encrypted;
          using (MemoryStream input = new MemoryStream(original))
          {
               using (MemoryStream output = new MemoryStream())
               {
                    this.EncryptToStreamWithPersonalKey(input, output, personalKey);
                    encrypted = output.ToArray();
               }
          }
          return encrypted;
     }
     
     private byte[] DecryptBufferWithPersonalKey(byte[] encrypted, string personalKey)
     {
          byte[] original;
          using (MemoryStream input = new MemoryStream(encrypted))
          {
               using (MemoryStream output = new MemoryStream())
               {
                    this.DecryptToStreamWithPersonalKey(input, output, personalKey);
                    original = output.ToArray();
               }
          }
          return original;
     }

     private void EncryptToStreamWithPersonalKey(Stream input, Stream output, string personalKey)
     {
          byte[] salt = null;
          byte[] iv = null;
          
          using (RijndaelManaged aes = new RijndaelManaged())
          {
               aes.KeySize = CryptographyProvider.EncryptionKeySize;
               aes.BlockSize = CryptographyProvider.EncryptionBlockSize;
               aes.Padding = PaddingMode.ISO10126;
               aes.Mode = CipherMode.CBC;

               aes.Key = this.RecalculateCipherPermutation(personalKey, ref salt, ref iv);
               aes.IV = iv;
               
               output.Write(salt, 0, CryptographyProvider.EncryptionKeySize / 8);
               output.Write(iv, 0, CryptographyProvider.EncryptionBlockSize / 8);
               
               this.EncryptToStream(input, output, aes.CreateEncryptor());
          }
     }

     private void DecryptToStreamWithPersonalKey(Stream input, Stream output, string personalKey)
     {
          byte[] salt = new byte[CryptographyProvider.EncryptionKeySize / 8];
          byte[] iv = new byte[CryptographyProvider.EncryptionBlockSize / 8];
          
          input.Read(salt, 0, CryptographyProvider.EncryptionKeySize / 8);
          input.Read(iv, 0, CryptographyProvider.EncryptionBlockSize / 8);

          using (RijndaelManaged aes = new RijndaelManaged())
          {
               aes.KeySize = CryptographyProvider.EncryptionKeySize;
               aes.BlockSize = CryptographyProvider.EncryptionBlockSize;
               aes.Padding = PaddingMode.ISO10126;
               aes.Mode = CipherMode.CBC;
               
               aes.Key = this.RecalculateCipherPermutation(personalKey, ref salt, ref iv);
               aes.IV = iv;
               
               this.DecryptFromStream(input, output, aes.CreateDecryptor());
          }
     }
     
     private byte[] RecalculateCipherPermutation(string personalKey, ref byte[] salt, ref byte[] iv)
     {
          byte[] permutation;
          if (salt == null || iv == null) // If new salt and vector is necessary (needed for every new encryption), leave salt and iv buffers null.
          {
               salt = new byte[CryptographyProvider.EncryptionKeySize / 8];
               iv = new byte[CryptographyProvider.EncryptionBlockSize / 8];
               
               using (RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider())
               {
                    rng.GetBytes(salt);
                    rng.GetBytes(iv);
               }
          }
          using (Rfc2898DeriveBytes pbkdf2 = new Rfc2898DeriveBytes(personalKey, salt, 10000))
          {
               permutation = pbkdf2.GetBytes(CryptographyProvider.EncryptionKeySize / 8);
          }
          return permutation;
     }

     private void EncryptToStream(Stream input, Stream output, ICryptoTransform encryptor)
     {
          using (CryptoStream stream = new CryptoStream(output, encryptor, CryptoStreamMode.Write))
          {
               byte[] buffer = new byte[CryptographyProvider.encryptionBufferSize];
               int bytesRead;
               while ((bytesRead = input.Read(buffer, 0, buffer.Length)) > 0)
               {
                    stream.Write(buffer, 0, bytesRead);
               }
               
               stream.FlushFinalBlock();
          }
     }

     private void DecryptFromStream(Stream input, Stream output, ICryptoTransform decryptor)
     {
          using (CryptoStream stream = new CryptoStream(input, decryptor, CryptoStreamMode.Read))
          {
               byte[] buffer = new byte[CryptographyProvider.encryptionBufferSize];
               try
               {
                    int bytesRead;
                    while ((bytesRead = stream.Read(buffer, 0, buffer.Length)) > 0)
                    {
                         output.Write(buffer, 0, bytesRead);
                    }   
               }
               catch (CryptographicException ex)
               {
                    Logger.Singleton.WriteLine("Stream could not be decrypted.");
               }
          }
     }
     
     #endregion
     
     #region Internal Hashing Functions

     private void HashToStream(Stream input, Stream output)
     {
          using (SHA256 sha = SHA256.Create())
          {
               byte[] hash = sha.ComputeHash(input);
               output.Write(hash, 0, hash.Length);
          }
     }
     
     #endregion
}