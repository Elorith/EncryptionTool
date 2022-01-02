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
    public static void Main(string[] args)
    {
        Program application = new Program();
        application.RunCommandLineInterface();
    }

    // Command-line interface implementation of the encryption tool.
    public void RunCommandLineInterface()
    {
        EncryptionTool tool = this.CreateCommandLineInterfaceTool();

        bool exitFlag = false;
        while (!exitFlag)
        {
            Logger.Singleton.WriteLine("Perform encryption or decryption? (1 = Encrypt, 2 = Decrypt)");
            ConsoleKeyInfo info = Console.ReadKey();
            Console.WriteLine();
            
            if (info.Key == ConsoleKey.D1)
            {
                Logger.Singleton.WriteLine("Please enter the path of the file to encrypt.");
                try
                {
                    tool.DoFileEncryption(Console.ReadLine());
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
                    tool.DoFileDecryption(Console.ReadLine());
                }
                catch (Exception ex)
                {
                    Logger.Singleton.WriteLine(ex.Message);
                }
            }
            
            Logger.Singleton.WriteLine("Exit now (Y/N)?");
            string response = Console.ReadLine();
            if (response != "Y" && response != "y")
            {
                continue;
            }
            exitFlag = true;
        }
    }
    
    private EncryptionTool CreateCommandLineInterfaceTool()
    {
        EncryptionTool tool = new EncryptionTool();
        
        tool.OnAskUserForEraseConfirmation += new EncryptionTool.OnAskUserForEraseConfirmationCallback((path) =>
        {
            Logger.Singleton.WriteLine("Are you sure you want to start erase of: '" + path + "' (Y/N)?");
            string response = Console.ReadLine();
            if (response != "Y" && response != "y")
            {
                return false;
            }

            return true;
        });
        
        tool.OnAskUserToEnterPasswordForEncryption += new EncryptionTool.OnAskUserToEnterPasswordForEncryptionCallback((path) =>
        {
            Logger.Singleton.WriteLine("'" + path + "' will be encrypted and securely erased. Please enter a password to encrypt with.");

            return Console.ReadLine();
        });
        
        tool.OnAskUserToRepeatPasswordForEncryption += new EncryptionTool.OnAskUserToRepeatPasswordForEncryptionCallback((path) =>
        {
            Logger.Singleton.WriteLine("Please re-enter the password.");

            return Console.ReadLine();
        });
        
        tool.OnUserEnteredNonMatchingPasswords += new EncryptionTool.OnUserEnteredNonMatchingPasswordsCallback(() =>
        {
            Logger.Singleton.WriteLine("Passwords do not match!");
        });
        
        tool.OnEncryptionVerificationProcessSuccess += new EncryptionTool.OnEncryptionVerificationProcessSuccessCallback(() =>
        {
            Logger.Singleton.WriteLine("Encryption verification process successful.");
        });
        
        tool.OnEncryptionAndSecureEraseProcessCompleted += new EncryptionTool.OnEncryptionAndSecureEraseProcessCompletedCallback(() =>
        {
            Logger.Singleton.WriteLine("Encryption and secure erase process successfully completed.");
        });
        
        tool.OnAskUserToEnterPasswordForDecryption += new EncryptionTool.OnAskUserToEnterPasswordForDecryptionCallback((path) =>
        {
            Logger.Singleton.WriteLine("'" + path + "' will be decrypted. Please enter the password originally used to encrypt with.");
            
            return Console.ReadLine();
        });
        
        tool.OnDecryptionProcessCompleted += new EncryptionTool.OnDecryptionProcessCompletedCallback(() =>
        {
            Logger.Singleton.WriteLine("Decryption process successfully completed.");
        });

        return tool;
    }
} 