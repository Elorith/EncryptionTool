using System.IO;
using System.Security.Cryptography;
using System.Text;

public class CryptographyProvider
{
     private void Encrypt(string path, string password)
     {
          using (RijndaelManaged aes = new RijndaelManaged())
          {
               aes.Padding = PaddingMode.ISO10126;
               aes.Mode = CipherMode.CBC;

               using (FileStream output = new FileStream(path + ".aes", FileMode.Create))
               {
                    byte[] salt = new byte[32];
                    byte[] iv = new byte[32];

                    using (RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider())
                    {
                         rng.GetBytes(salt);
                         rng.GetBytes(iv);
                    }
                    output.Write(salt, 0, 32);
                    output.Write(iv, 0, 32);
                    
                    using (Rfc2898DeriveBytes key = new Rfc2898DeriveBytes(password, salt))
                    {
                         aes.Key = key.GetBytes(32);
                         aes.IV = iv;
                    }

                    using (CryptoStream stream = new CryptoStream(output, aes.CreateEncryptor(), CryptoStreamMode.Write))
                    {
                         using (FileStream input = new FileStream(path, FileMode.Open))
                         {
                              byte[] buffer = new byte[1048576];
                              int bytesRead;
                              while ((bytesRead = input.Read(buffer, 0, buffer.Length)) > 0)
                              {
                                   stream.Write(buffer, 0, bytesRead);
                              }
                         }
                    }
               }
          }
     }

     private void Decrypt(string path, string password)
     {
          using (RijndaelManaged aes = new RijndaelManaged())
          {
               aes.Padding = PaddingMode.ISO10126;
               aes.Mode = CipherMode.CBC;
               
               using (FileStream input = new FileStream(path, FileMode.Open))
               {
                    byte[] salt = new byte[32];
                    byte[] iv = new byte[32];

                    input.Read(salt, 0, 32);
                    input.Read(iv, 0, 32);
                    
                    using (Rfc2898DeriveBytes key = new Rfc2898DeriveBytes(password, salt))
                    {
                         aes.Key = key.GetBytes(32);
                         aes.IV = iv;
                    }

                    using (CryptoStream stream = new CryptoStream(input, aes.CreateDecryptor(), CryptoStreamMode.Read))
                    {
                         using (FileStream output = new FileStream(Path.GetFileNameWithoutExtension(path), FileMode.Create))
                         {
                              byte[] buffer = new byte[1048576];
                              int bytesRead;
                              while ((bytesRead = stream.Read(buffer, 0, buffer.Length)) > 0)
                              {
                                   output.Write(buffer, 0, bytesRead);
                              }   
                         }    
                    }
               }
          }
     }
}