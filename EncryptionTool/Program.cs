using System;
using System.IO;

public class Program
{
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

        string path1 = Console.ReadLine();
        application.DoEncryption(path1);

        string path2 = Console.ReadLine();
        application.DoDecryption(path2);
    }
    
    public void DoEncryption(string path)
    {
        Logger.Singleton.WriteLine("'" + path + "' will be encrypted. Please provide a password to encrypt with.");
        string response = Console.ReadLine();
        
        Logger.Singleton.WriteLine("Please re-enter the password.");
        string response2 = Console.ReadLine();

        if (response != response2)
        {
            Logger.Singleton.WriteLine("Passwords do not match!");
            return;
        }

        CryptographyProvider cryptography = new CryptographyProvider();
        cryptography.EncryptFileWithPersonalKey(path, response);
        
        this.DoSecureErase(path, SanitisationAlgorithmType.DoDSensitive, false);
    }
    
    public void DoDecryption(string path)
    {
        Logger.Singleton.WriteLine("'" + path + "' will be decrypted. Please enter the password originally used to encrypt with.");
        
        string response = Console.ReadLine();

        CryptographyProvider cryptography = new CryptographyProvider();
        cryptography.DecryptFileWithPersonalKey(path, response);
        
        File.Delete(path);
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