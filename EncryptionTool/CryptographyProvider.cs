using System.IO;
using System.Security.Cryptography;
using System.Text;

public class CryptographyProvider
{
     public const int KeySize = 256;
     public const int BlockSize = 128;
     private const ulong encryptionBufferSize = 1048576;
     
     public void EncryptFileWithPersonalKey(string path, string personalKey)
     {
          using (FileStream input = new FileStream(path, FileMode.Open))
          {
               string outputPath = this.GetPathForEncryptedFile(path);
               using (FileStream output = new FileStream(outputPath, FileMode.Create))
               {
                    this.EncryptWithPersonalKey(input, output, personalKey);
               }
          }
          
          Logger.Singleton.WriteLine("'" + path + "' has been successfully encrypted.");
     }

     public void DecryptFileWithPersonalKey(string path, string personalKey)
     {
          using (FileStream input = new FileStream(path, FileMode.Open))
          {
               string outputPath = this.GetPathForOriginalFile(path);
               using (FileStream output = new FileStream(outputPath, FileMode.Create))
               {
                    this.DecryptWithPersonalKey(input, output, personalKey);
               }
          }

          Logger.Singleton.WriteLine("'" + path + "' has been successfully decrypted.");
     }

     private string GetPathForEncryptedFile(string originalPath)
     {
          return Path.Combine(originalPath, ".aes");
     }
     
     private string GetPathForOriginalFile(string encryptedPath)
     {
          return Path.Combine(Path.GetDirectoryName(encryptedPath), Path.GetFileNameWithoutExtension(encryptedPath));
     }

     public string EncryptStringWithPersonalKey(string original, string personalKey)
     {
          string encrypted;
          using (MemoryStream input = new MemoryStream(Encoding.UTF8.GetBytes(original)))
          {
               using (MemoryStream output = new MemoryStream())
               {
                    this.EncryptWithPersonalKey(input, output, personalKey);
                    encrypted = Encoding.ASCII.GetString(output.ToArray());
               }
          }
          return encrypted;
     }

     public string DecryptStringWithPersonalKey(string encrypted, string personalKey)
     {
          string original;
          using (MemoryStream input = new MemoryStream(Encoding.UTF8.GetBytes(encrypted)))
          {
               using (MemoryStream output = new MemoryStream())
               {
                    this.DecryptWithPersonalKey(input, output, personalKey);
                    original = Encoding.ASCII.GetString(output.ToArray());
               }
          }
          return original;
     }

     private byte[] CalculateCipherFromPersonalKey(string personalKey, ref byte[] salt, ref byte[] iv)
     {
          byte[] cipher;
          if (salt == null || iv == null) // If new salt and vector is necessary (needed for every new encryption), leave salt and iv buffers null.
          {
               salt = new byte[CryptographyProvider.KeySize / 8];
               iv = new byte[CryptographyProvider.BlockSize / 8];
               
               using (RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider())
               {
                    rng.GetBytes(salt);
                    rng.GetBytes(iv);
               }
          }
          using (Rfc2898DeriveBytes pbkdf2 = new Rfc2898DeriveBytes(personalKey, salt))
          {
               cipher = pbkdf2.GetBytes(CryptographyProvider.KeySize / 8);
          }
          return cipher;
     }

     private void EncryptWithPersonalKey(Stream input, Stream output, string personalKey)
     {
          byte[] salt = null;
          byte[] iv = null;
          
          using (RijndaelManaged aes = new RijndaelManaged())
          {
               aes.KeySize = CryptographyProvider.KeySize;
               aes.BlockSize = CryptographyProvider.BlockSize;
               aes.Padding = PaddingMode.ISO10126;
               aes.Mode = CipherMode.CBC;

               aes.Key = this.CalculateCipherFromPersonalKey(personalKey, ref salt, ref iv);
               aes.IV = iv;
               
               output.Write(salt, 0, CryptographyProvider.KeySize / 8);
               output.Write(iv, 0, CryptographyProvider.BlockSize / 8);
               
               this.EncryptToStream(input, output, aes.CreateEncryptor());
          }
     }

     private void DecryptWithPersonalKey(Stream input, Stream output, string personalKey)
     {
          byte[] salt = new byte[CryptographyProvider.KeySize / 8];
          byte[] iv = new byte[CryptographyProvider.BlockSize / 8];
          
          input.Read(salt, 0, CryptographyProvider.KeySize / 8);
          input.Read(iv, 0, CryptographyProvider.BlockSize / 8);

          using (RijndaelManaged aes = new RijndaelManaged())
          {
               aes.KeySize = CryptographyProvider.KeySize;
               aes.BlockSize = CryptographyProvider.BlockSize;
               aes.Padding = PaddingMode.ISO10126;
               aes.Mode = CipherMode.CBC;
               
               aes.Key = this.CalculateCipherFromPersonalKey(personalKey, ref salt, ref iv);
               aes.IV = iv;
               
               this.DecryptFromStream(input, output, aes.CreateDecryptor());
          }
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
}