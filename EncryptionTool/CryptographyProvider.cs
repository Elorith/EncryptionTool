using System.IO;
using System.Security.Cryptography;
using System.Text;

public class CryptographyProvider
{
     private void Encrypt(string path, string password)
     {
          byte[] salt = new byte[32];
          using (RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider())
          {
               rng.GetBytes(salt);
          }

          using (RijndaelManaged aes = new RijndaelManaged())
          {
               aes.Padding = PaddingMode.ISO10126;
               aes.Mode = CipherMode.CBC;
               
               using (Rfc2898DeriveBytes key = new Rfc2898DeriveBytes(password, salt))
               {
                    aes.Key = key.GetBytes(32);
                    aes.GenerateIV();    
               }

               using (FileStream output = new FileStream(path + ".aes", FileMode.Create))
               {
                    output.Write(salt, 0, salt.Length);
                    output.Write(aes.IV, 0, aes.IV.Length);
               
                    using (CryptoStream stream = new CryptoStream(output, aes.CreateEncryptor(), CryptoStreamMode.Write))
                    {
                         using (FileStream input = new FileStream(path, FileMode.Open))
                         {
                              byte[] buffer = new byte[1048576];
                              int read;
                              while ((read = input.Read(buffer, 0, buffer.Length)) > 0)
                              {
                                   stream.Write(buffer, 0, read);
                              }
                         }
                    }
               }
          }
     }
}