using System;

public class Program
{
    /*
     * 1) Take original file and encrypt using the user's personal key (which needs to be entered each time a security related action is taken).
     * 2) Write encrypted variation of the file to disk.
     * 3) Verify that the newly written file is valid by decrypting it from disk and checking it is identical.
     * 4) Securely release user key from memory (ZeroMemory).
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
            
            string[] split = command.Split(new []{' '},2);
            if (string.Equals(split[0], "encrypt", StringComparison.OrdinalIgnoreCase))
            {
                try
                {
                    application.DoFileEncryption(split[1].Trim('"'));
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
                    application.DoFileDecryption(split[1].Trim('"'));
                }
                catch (Exception ex)
                {
                    Logger.Singleton.WriteLine(ex.Message);
                }
            }
            else if (string.Equals(split[0], "exit", StringComparison.OrdinalIgnoreCase))
            {
                break;
            }
            else
            {
                Logger.Singleton.WriteLine("Specified command was not recognised.");
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

            return this.ReadPasswordFromConsoleLine();
        });
        
        application.OnAskUserToRepeatPasswordForEncryption += new EncryptionTool.OnAskUserToRepeatPasswordForEncryptionCallback((path) =>
        {
            Logger.Singleton.WriteLine("Please re-enter the password.");

            return this.ReadPasswordFromConsoleLine();
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
            
            return this.ReadPasswordFromConsoleLine();
        });
        
        application.OnDecryptionProcessCompleted += new EncryptionTool.OnDecryptionProcessCompletedCallback(() =>
        {
            Logger.Singleton.WriteLine("Decryption process successfully completed.");
        });

        return application;
    }

    private string ReadPasswordFromConsoleLine()
    {
        string input = string.Empty;
        
        ConsoleKey key;
        do
        {
            var keyInfo = Console.ReadKey(true);
            key = keyInfo.Key;

            if (key == ConsoleKey.Backspace && input.Length > 0)
            {
                Console.Write("\b \b");
                input = input.Remove(input.Length - 1, 1);
            }
            else if (!char.IsControl(keyInfo.KeyChar))
            {
                Console.Write("*");
                input += keyInfo.KeyChar;
            }
        }
        while (key != ConsoleKey.Enter);

        Console.Write("\n");
        
        return input;
    }
} 