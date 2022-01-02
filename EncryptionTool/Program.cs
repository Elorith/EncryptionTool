using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Security.Cryptography;

public class Program
{
    // Command-line interface implementation of the encryption tool.
    public static void Main(string[] args)
    {
        /*
         * 1) Take original file and encrypt using the user's personal key (which needs to be entered each time a security related action is taken).
         * 2) Write encrypted variation of the file to disk.
         * 3) Verify that the newly written file is valid by decrypting it from disk and checking it is identical.
         * 4) Securely erase user key from memory (ZeroMemory).
         * 5) Securely erase the original file from disk using implementation based on US DoD 5220.22-M (ECE).
         */
        
        Program application = new Program();

        bool exitFlag = false;
        while (!exitFlag)
        {
            Logger.Singleton.WriteLine("Perform encryption or decryption? (Encrypt = 1; Decrypt = 2)");
            ConsoleKeyInfo info = Console.ReadKey();
            Console.WriteLine();
            
            if (info.Key == ConsoleKey.D1)
            {
                Logger.Singleton.WriteLine("Please enter the path of the file to encrypt.");
                try
                {
                    application.DoFileEncryption(Console.ReadLine());
                }
                catch (Exception ex)
                {
                    Logger.Singleton.WriteLine(ex.Message);
                }
            }
            else if (info.Key == ConsoleKey.D2)
            {
                Logger.Singleton.WriteLine("Please enter the path of the file to decrypt.");
                try
                {
                    application.DoFileDecryption(Console.ReadLine());
                }
                catch (Exception ex)
                {
                    Logger.Singleton.WriteLine(ex.Message);
                }
            }
            
            Logger.Singleton.WriteLine("Exit now (Y/N)?");
            string response = Console.ReadLine();
            if (response != "Y" || response != "y")
            {
                continue;
            }
            exitFlag = true;
        }
    }
    
    public void DoSecureErase(string path, SanitisationAlgorithmType type, bool askForConfirmation = true)
    {
        if (askForConfirmation)
        {
            Logger.Singleton.WriteLine("Are you sure you want to start erase of: '" + path + "' (Y/N)?");
            string response = Console.ReadLine();
            if (response != "Y" || response != "y")
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
        
        Program.ZeroMemory(response2Handle.AddrOfPinnedObject(), response2.Length * 2);
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

        Program.ZeroMemory(responseHandle.AddrOfPinnedObject(), response.Length * 2);
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
        
        Program.ZeroMemory(responseHandle.AddrOfPinnedObject(), response.Length * 2);
        responseHandle.Free();
        
        File.Delete(path);
        
        Logger.Singleton.WriteLine("Decryption process successfully completed.");
    }
} 