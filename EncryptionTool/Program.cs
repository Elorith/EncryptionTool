using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

public class Program
{
    /*
     * 1) Take original file and encrypt using the user's personal key (which needs to be entered each time a security related action is taken).
     * 2) Write encrypted variation of the file to disk.
     * 3) Verify that the newly written file is valid by decrypting it from disk and checking it is identical.
     * 4) Securely release sensitive data from memory (ZeroMemory).
     * 5) Securely erase the original file from disk using implementation based on US DoD 5220.22-M (ECE).
     */
    public static void Main(string[] args) => new Program().RunCommandLineInterface();

    // Command-line interface implementation of the encryption tool.
    public void RunCommandLineInterface()
    {
        Console.Title = "Encryption Tool";

        EncryptionTool application = this.CreateCommandLineInterfaceTool();
        
        application.DoDirectoryEncryption(Console.ReadLine());

        while (true)
        {
            string[] command = this.ReadCommandFromConsoleLine();
            
            if (string.Equals(command[0], "exit", StringComparison.OrdinalIgnoreCase))
            {
                break;
            }
            
            try
            {
                if (string.Equals(command[0], "encrypt", StringComparison.OrdinalIgnoreCase))
                {
                    application.DoFileEncryption(command[1].Trim('"'));
                }
                else if (string.Equals(command[0], "decrypt", StringComparison.OrdinalIgnoreCase))
                {
                    application.DoFileDecryption(command[1].Trim('"'));
                }
                else if (string.Equals(command[0], "erase", StringComparison.OrdinalIgnoreCase))
                {
                    application.DoSecureErase(command[1].Trim('"'), SanitisationAlgorithmType.DoDSensitive, true);
                }
                else
                {
                    Logger.Singleton.WriteLine("Specified command was not recognised.");
                }
            }
            catch (Exception ex)
            {
                Logger.Singleton.WriteLine(ex.Message);
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
        
        application.OnEncryptionProcessCompleted += new EncryptionTool.OnEncryptionProcessCompletedCallback(() =>
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

    private string[] ReadCommandFromConsoleLine()
    {
        string input = Console.ReadLine();
        if (input == null)
        {
            return null;
        }
        
        List<string> command = new List<string>();

        Regex splitter = new Regex(@"[\""].+?[\""]|[^ ]+", RegexOptions.Compiled);
        foreach (Match match in splitter.Matches(input))
        {
            if (match.Value.Length == 0)
            {
                command.Add("");
            }
            else
            {
                command.Add(match.Value.TrimStart(','));
            }
        }

        return command.ToArray();
    }

    private string ReadPasswordFromConsoleLine()
    {
        string input = string.Empty;
        
        ConsoleKey key;
        do
        {
            ConsoleKeyInfo keyInfo = Console.ReadKey(true);
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