using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

public class CryptographyProvider
{
     public const int EncryptionKeySize = 256;
     public const int EncryptionBlockSize = 128;
     private const ulong encryptionBufferSize = 1048576;

     #region Public API Functions
     
     public string EncryptStringWithPersonalKey(string original, string personalKey)
     {
          byte[] buffer = this.EncryptStringToBufferWithPersonalKey(original, personalKey);
          string encrypted = this.BufferToHexadecimal(buffer);

          return encrypted;
     }
     
     public string DecryptStringWithPersonalKey(string encrypted, string personalKey)
     {
          byte[] buffer = this.HexadecimalToBuffer(encrypted);
          string original = this.DecryptStringFromBufferWithPersonalKey(buffer, personalKey);

          return original;
     }
     
     public byte[] EncryptStringToBufferWithPersonalKey(string original, string personalKey)
     {
          byte[] encrypted = this.EncryptBufferWithPersonalKey(Encoding.UTF8.GetBytes(original), personalKey);

          return encrypted;
     }

     public string DecryptStringFromBufferWithPersonalKey(byte[] encrypted, string personalKey)
     {
          string original = Encoding.UTF8.GetString(this.DecryptBufferWithPersonalKey(encrypted, personalKey));

          return original;
     }

     public string EncryptFileToDiskWithPersonalKey(string path, string personalKey)
     {
          string outputPath;
          using (FileStream input = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read))
          {
               string encryptedFileName = this.HashFileToString(path) + ".aes";
               string directoryName = Path.GetDirectoryName(path);

               outputPath = Path.Combine(directoryName, encryptedFileName);
               using (FileStream output = new FileStream(outputPath, FileMode.Create))
               {
                    string originalFileName = Path.GetFileName(path);
                    this.EncryptHeaderToStream(originalFileName, output, personalKey);
                    
                    this.EncryptBodyToStream(input, output, personalKey);
               }
          }

          Logger.Singleton.WriteLine("'" + path + "' has been successfully encrypted to disk.");
          return outputPath;
     }

     public string DecryptFileToDiskWithPersonalKey(string path, string personalKey)
     {
          string outputPath;
          using (FileStream input = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read))
          {
               string originalFileName = this.DecryptHeaderFromStream(input, personalKey);

               outputPath = Path.Combine(Path.GetDirectoryName(path), originalFileName);
               using (FileStream output = new FileStream(outputPath, FileMode.Create))
               {
                    this.DecryptBodyFromStream(input, output, personalKey);
               }
          }

          Logger.Singleton.WriteLine("'" + path + "' has been successfully decrypted to disk.");
          return outputPath;
     }
     
     public byte[] EncryptFileToMemoryWithPersonalKey(string path, string personalKey)
     {
          byte[] buffer;
          using (FileStream input = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read))
          {
               using (MemoryStream output = new MemoryStream())
               {
                    this.EncryptBodyToStream(input, output, personalKey);
                    buffer = output.ToArray();
               }
          }
          
          return buffer;
     }

     public byte[] DecryptFileToMemoryWithPersonalKey(string path, string personalKey)
     {
          byte[] buffer;
          using (FileStream input = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read))
          {
               this.DecryptHeaderFromStream(input, personalKey);

               using (MemoryStream output = new MemoryStream())
               {
                    this.DecryptBodyFromStream(input, output, personalKey);
                    buffer = output.ToArray();
               }
          }

          return buffer;
     }

     public string HashFileToString(string path)
     {
          string hash;
          using (FileStream input = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read))
          { 
               hash = this.HashToString(input, false);
          }

          return hash;
     }
     
     public string HashBufferToString(byte[] buffer)
     {
          string hash;
          using (MemoryStream stream = new MemoryStream(buffer))
          { 
               hash = this.HashToString(stream, false);
          }

          return hash;
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
                    this.EncryptBodyToStream(input, output, personalKey);
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
                    this.DecryptBodyFromStream(input, output, personalKey);
                    original = output.ToArray();
               }
          }
          return original;
     }

     private void EncryptHeaderToStream(string header, Stream output, string personalKey)
     {
          byte[] headerBytes = this.EncryptStringToBufferWithPersonalKey(header, personalKey);
          byte[] headerLengthBytes = BitConverter.GetBytes(headerBytes.Length);

          output.Write(headerLengthBytes, 0, 4);
          output.Write(headerBytes, 0, headerBytes.Length);
     }

     private string DecryptHeaderFromStream(Stream input, string personalKey)
     {
          byte[] headerLengthBytes = new byte[4];
          input.Read(headerLengthBytes, 0, 4);

          byte[] headerBytes = new byte[BitConverter.ToInt32(headerLengthBytes, 0)];
          input.Read(headerBytes, 0, headerBytes.Length);
          
          return this.DecryptStringFromBufferWithPersonalKey(headerBytes, personalKey);
     }

     private void EncryptBodyToStream(Stream input, Stream output, string personalKey)
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

     private void DecryptBodyFromStream(Stream input, Stream output, string personalKey)
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

     private string HashToString(Stream input, bool useUniqueCipherPermutation)
     {
          string hash;
          using (MemoryStream output = new MemoryStream())
          {
               this.HashToStream(input, output, useUniqueCipherPermutation);
               byte[] buffer = output.ToArray();

               hash = this.BufferToHexadecimal(buffer);
          }
          return hash;
     }

     private void HashToStream(Stream input, Stream output, bool useUniqueCipherPermutation)
     {
          byte[] permutation;
          if (useUniqueCipherPermutation)
          {
               /*byte[] unique = new byte[16];
               using (RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider())
               {
                    rng.GetBytes(unique);
               }
    
               permutation = this.RecalculateCipherPermutation(Encoding.UTF8.GetString(unique));*/
               // TODO: Figure out how to implement this.    
          }
          using (SHA256 sha = SHA256.Create())
          {
               byte[] hash = sha.ComputeHash(input);
               output.Write(hash, 0, hash.Length);
          }
     }
     
     #endregion
     
     #region Miscellaneous Internal Functions

     private byte[] RecalculateCipherPermutation(string unique, ref byte[] salt, ref byte[] iv)
     {
          byte[] permutation;
          if (salt == null || iv == null) // If new salt and vector is needed, leave salt and iv buffers null.
          {
               salt = new byte[CryptographyProvider.EncryptionKeySize / 8];
               iv = new byte[CryptographyProvider.EncryptionBlockSize / 8];
               
               using (RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider())
               {
                    rng.GetBytes(salt);
                    rng.GetBytes(iv);
               }
          }
          using (Rfc2898DeriveBytes pbkdf2 = new Rfc2898DeriveBytes(unique, salt, 10000))
          {
               permutation = pbkdf2.GetBytes(CryptographyProvider.EncryptionKeySize / 8);
          }
          return permutation;
     }

     private string BufferToHexadecimal(byte[] buffer)
     {
          StringBuilder builder = new StringBuilder();
          for (int index = 0; index < buffer.Length; index++)
          {
               byte value = buffer[index];
               builder.Append(value.ToString("x2"));  
          }
          
          return builder.ToString();
     }
     
     private byte[] HexadecimalToBuffer(string hex)
     {
          int length = hex.Length;
          
          byte[] buffer = new byte[length / 2];
          for (int index = 0; index < length; index += 2)
          {
               buffer[index / 2] = Convert.ToByte(hex.Substring(index, 2), 16);
          }

          return buffer;
     }
     
     #endregion
}