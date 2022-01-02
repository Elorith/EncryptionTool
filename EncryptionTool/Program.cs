using System;

public class Program
{
    /*
     * 1) Take original file and encrypt using the user's personal key (which needs to be entered each time a security related action is taken).
     * 2) Write encrypted variation of the file to disk.
     * 3) Verify that the newly written file is valid by decrypting it from disk and checking it is identical.
     * 4) Securely erase user key from memory (ZeroMemory).
     * 5) Securely erase the original file from disk using implementation based on US DoD 5220.22-M (ECE).
     */
    public static void Main(string[] args) => new Program().RunCommandLineInterface();

    // Command-line interface implementation of the encryption tool.
    public void RunCommandLineInterface()
    {
        Console.Title = "Encryption Tool";

        EncryptionTool application = this.CreateCommandLineInterfaceTool();

        while (true)
        {
            string command = Console.ReadLine();
            if (command == null)
            {
                continue;
            }
            
            string[] split = command.Split(' ');
            if (string.Equals(split[0], "encrypt", StringComparison.OrdinalIgnoreCase))
            {
                try
                {
                    application.DoFileEncryption(split[1]);
                }
                catch (Exception ex)
                {
                    Logger.Singleton.WriteLine(ex.Message);
                }
            }
            else if (string.Equals(split[0], "decrypt", StringComparison.OrdinalIgnoreCase))
            {
                try
                {
                    application.DoFileDecryption(split[1]);
                }
                catch (Exception ex)
                {
                    Logger.Singleton.WriteLine(ex.Message);
                }
            }
        }
    }
    
    private EncryptionTool CreateCommandLineInterfaceTool()
    {
        EncryptionTool application = new EncryptionTool();
        
        application.OnAskUserForEraseConfirmation += new EncryptionTool.OnAskUserForEraseConfirmationCallback((path) =>
        {
            Logger.Singleton.WriteLine("Are you sure you want to start erase of: '" + path + "' (Y/N)?");
            string response = Console.ReadLine();
            if (response != "Y" && response != "y")
            {
                return false;
            }

            return true;
        });
        
        application.OnAskUserToEnterPasswordForEncryption += new EncryptionTool.OnAskUserToEnterPasswordForEncryptionCallback((path) =>
        {
            Logger.Singleton.WriteLine("'" + path + "' will be encrypted and securely erased. Please enter a password to encrypt with.");

            return Console.ReadLine();
        });
        
        application.OnAskUserToRepeatPasswordForEncryption += new EncryptionTool.OnAskUserToRepeatPasswordForEncryptionCallback((path) =>
        {
            Logger.Singleton.WriteLine("Please re-enter the password.");

            return Console.ReadLine();
        });
        
        application.OnUserEnteredNonMatchingPasswords += new EncryptionTool.OnUserEnteredNonMatchingPasswordsCallback(() =>
        {
            Logger.Singleton.WriteLine("Passwords do not match!");
        });
        
        application.OnEncryptionVerificationProcessSuccess += new EncryptionTool.OnEncryptionVerificationProcessSuccessCallback(() =>
        {
            Logger.Singleton.WriteLine("Encryption verification process successful.");
        });
        
        application.OnEncryptionAndSecureEraseProcessCompleted += new EncryptionTool.OnEncryptionAndSecureEraseProcessCompletedCallback(() =>
        {
            Logger.Singleton.WriteLine("Encryption and secure erase process successfully completed.");
        });
        
        application.OnAskUserToEnterPasswordForDecryption += new EncryptionTool.OnAskUserToEnterPasswordForDecryptionCallback((path) =>
        {
            Logger.Singleton.WriteLine("'" + path + "' will be decrypted. Please enter the password originally used to encrypt with.");
            
            return Console.ReadLine();
        });
        
        application.OnDecryptionProcessCompleted += new EncryptionTool.OnDecryptionProcessCompletedCallback(() =>
        {
            Logger.Singleton.WriteLine("Decryption process successfully completed.");
        });

        return application;
    }
} 