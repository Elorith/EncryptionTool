using System;
using System.IO;

public class Program
{
    public static void Main(string[] args)
    {
        /*
         * 1) Take original file and encrypt using the user's personal key (which needs to be entered each time a security related action is taken).
         * 2) Write encrypted variation of the file to disk.
         * 3) Verify that the newly written file is valid.
         * 4) Securely erase the original file from disk using implementation of US DoD 5220.22-M (ECE).
         */
        
        Program application = new Program();

        string path1 = Console.ReadLine();
        application.DoEncryption(path1);
        application.DoSecureErase(path1, SanitisationAlgorithmType.DoDSensitive, false);
        
        string path2 = Console.ReadLine();
        application.DoDecryption(path2);
        File.Delete(path2);
    }
    
    public void DoEncryption(string path)
    {
        Logger.Singleton.WriteLine("'" + path + "' will be encrypted. Please provide a password to encrypt with.");
        
        string response = Console.ReadLine();

        CryptographyProvider cryptography = new CryptographyProvider();
        cryptography.EncryptFileWithPersonalKey(path, response);
    }
    
    public void DoDecryption(string path)
    {
        Logger.Singleton.WriteLine("'" + path + "' will be decrypted. Please enter the password originally used to encrypt with.");
        
        string response = Console.ReadLine();

        CryptographyProvider cryptography = new CryptographyProvider();
        cryptography.DecryptFileWithPersonalKey(path, response);
    }
    
    public void DoSecureErase(string path, SanitisationAlgorithmType type, bool askForConfirmation = true)
    {
        if (askForConfirmation)
        {
            Logger.Singleton.WriteLine("Are you sure you want to start erase of: '" + path + "' (Y/N)?");
        
            string response = Console.ReadLine();
            if (response != "Y")
            {
                return;
            }   
        }

        SecureEraser eraser = new SecureEraser();
        eraser.ErasePath(path, type);
    }
} 