using System.IO;
using System.Security.Cryptography;

public class CryptographyProvider
{
     private const ulong encryptionBufferSize = 1048576;
     
     public void Encrypt(string path, string password)
     {
          using (RijndaelManaged aes = new RijndaelManaged())
          {
               aes.Padding = PaddingMode.ISO10126;
               aes.Mode = CipherMode.CBC;

               using (FileStream input = new FileStream(path, FileMode.Open))
               {
                    string outputPath = path + ".aes";
                    using (FileStream output = new FileStream(outputPath, FileMode.Create))
                    {
                         byte[] salt = new byte[32];
                         byte[] iv = new byte[16];

                         using (RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider())
                         {
                              rng.GetBytes(salt);
                              rng.GetBytes(iv);
                         }
                         output.Write(salt, 0, 32);
                         output.Write(iv, 0, 16);
                    
                         using (Rfc2898DeriveBytes key = new Rfc2898DeriveBytes(password, salt))
                         {
                              aes.Key = key.GetBytes(32);
                              aes.IV = iv;
                         }
                         
                         this.Encrypt(input, output, aes.CreateEncryptor());
                    }
               }
          }
          
          Logger.Singleton.WriteLine("'" + path + " has been successfully encrypted.");
     }

     public void Decrypt(string path, string password)
     {
          using (RijndaelManaged aes = new RijndaelManaged())
          {
               aes.Padding = PaddingMode.ISO10126;
               aes.Mode = CipherMode.CBC;
               
               using (FileStream input = new FileStream(path, FileMode.Open))
               {
                    string outputPath = Path.Combine(Path.GetDirectoryName(path), Path.GetFileNameWithoutExtension(path));
                    using (FileStream output = new FileStream(outputPath, FileMode.Create))
                    {
                         byte[] salt = new byte[32];
                         byte[] iv = new byte[16];

                         input.Read(salt, 0, 32);
                         input.Read(iv, 0, 16);
                    
                         using (Rfc2898DeriveBytes key = new Rfc2898DeriveBytes(password, salt))
                         {
                              aes.Key = key.GetBytes(32);
                              aes.IV = iv;
                         }
                         
                         this.Decrypt(input, output, aes.CreateDecryptor());
                    }
               }
          }
          
          Logger.Singleton.WriteLine("'" + path + " has been successfully decrypted.");
     }

     private void Encrypt(Stream input, Stream output, ICryptoTransform encryptor)
     {
          using (CryptoStream stream = new CryptoStream(output, encryptor, CryptoStreamMode.Write))
          {
               byte[] buffer = new byte[CryptographyProvider.encryptionBufferSize];
               int bytesRead;
               while ((bytesRead = input.Read(buffer, 0, buffer.Length)) > 0)
               {
                    stream.Write(buffer, 0, bytesRead);
               }
          }
     }

     private void Decrypt(Stream input, Stream output, ICryptoTransform decryptor)
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