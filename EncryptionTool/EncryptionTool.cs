using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Security.Cryptography;

public class EncryptionTool
{
    public void DoSecureErase(string path, SanitisationAlgorithmType type, bool askForConfirmation = true)
    {
        if (askForConfirmation)
        {
            Logger.Singleton.WriteLine("Are you sure you want to start erase of: '" + path + "' (Y/N)?");
            string response = Console.ReadLine();
            if (response != "Y" && response != "y")
            {
                return;
            }   
        }

        SecureEraser eraser = new SecureEraser();
        eraser.ErasePath(path, type);
    }
    
    [DllImport("Kernel32.dll", EntryPoint = "RtlZeroMemory")]
    public static extern bool ZeroMemory(IntPtr destination, int length);

    public void DoFileEncryption(string path)
    {
        if (!File.Exists(path))
        {
            throw new ArgumentException("Specified path is not a file or does not exist");
        }
        
        Logger.Singleton.WriteLine("'" + path + "' will be encrypted and securely erased. Please enter a password to encrypt with.");
        string response = Console.ReadLine();
        GCHandle responseHandle = GCHandle.Alloc(response, GCHandleType.Pinned);

        Logger.Singleton.WriteLine("Please re-enter the password.");
        string response2 = Console.ReadLine();
        GCHandle response2Handle = GCHandle.Alloc(response2, GCHandleType.Pinned);

        if (response != response2)
        {
            Logger.Singleton.WriteLine("Passwords do not match!");
            return;
        }
        
        EncryptionTool.ZeroMemory(response2Handle.AddrOfPinnedObject(), response2.Length * 2);
        response2Handle.Free();

        CryptographyProvider cryptography = new CryptographyProvider();
        string outputPath = cryptography.EncryptFileToDiskWithPersonalKey(path, response);
        
        try
        {
            byte[] decrypted = cryptography.DecryptFileToMemoryWithPersonalKey(outputPath, response);
            string hash = cryptography.HashBufferToString(decrypted);

            if (hash != Path.GetFileNameWithoutExtension(outputPath))
            {
                throw new CryptographicException("Encryption verification process failed");
            }
        }
        catch (Exception ex)
        {
            throw new CryptographicException("Encryption verification process failed");
        }
        Logger.Singleton.WriteLine("Encryption verification process successful.");

        EncryptionTool.ZeroMemory(responseHandle.AddrOfPinnedObject(), response.Length * 2);
        responseHandle.Free();

        this.DoSecureErase(path, SanitisationAlgorithmType.DoDSensitive, false);
        
        Logger.Singleton.WriteLine("Encryption and secure erase process successfully completed.");
    }
    
    public void DoFileDecryption(string path)
    {
        if (!File.Exists(path))
        {
            throw new ArgumentException("Specified path is not a file or does not exist");
        }
        
        Logger.Singleton.WriteLine("'" + path + "' will be decrypted. Please enter the password originally used to encrypt with.");
        
        string response = Console.ReadLine();
        GCHandle responseHandle = GCHandle.Alloc(response, GCHandleType.Pinned); 

        CryptographyProvider cryptography = new CryptographyProvider();
        cryptography.DecryptFileToDiskWithPersonalKey(path, response);
        
        EncryptionTool.ZeroMemory(responseHandle.AddrOfPinnedObject(), response.Length * 2);
        responseHandle.Free();
        
        File.Delete(path);
        
        Logger.Singleton.WriteLine("Decryption process successfully completed.");
    }
}