using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;

public class ConsoleLineApplication : EncryptionTool
{
    public void RunTool()
    {
        while (true)
        {
            string[] command = this.ReadCommandFromConsoleLine();
            
            if (string.Equals(command[0], "exit", StringComparison.OrdinalIgnoreCase))
            {
                return;
            }
            
            try
            {
                string path = command[1].Trim('"');
                bool isDirectory = (File.GetAttributes(path) & FileAttributes.Directory) == FileAttributes.Directory;
                
                if (string.Equals(command[0], "encrypt", StringComparison.OrdinalIgnoreCase))
                {
                    if (!isDirectory)
                    {
                        this.DoFileEncryption(path);
                    }
                    else
                    {
                        this.DoDirectoryEncryption(path);
                    }
                }
                else if (string.Equals(command[0], "decrypt", StringComparison.OrdinalIgnoreCase))
                {
                    if (!isDirectory)
                    {
                        this.DoFileDecryption(path);
                    }
                    else
                    {
                        this.DoDirectoryDecryption(path);
                    }
                }
                else if (string.Equals(command[0], "erase", StringComparison.OrdinalIgnoreCase))
                {
                    this.DoSecureErase(path, SanitisationAlgorithmType.DoDSensitive, true);
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
    
        protected override string AskUserToEnterPasswordForEncryption(string path)
    {
        Logger.Singleton.WriteLine("'" + path + "' will be encrypted and securely erased. Please enter a password to encrypt with.");

        return this.ReadPasswordFromConsoleLine();
    }

    protected override string AskUserToRepeatPasswordForEncryption(string path)
    {
        Logger.Singleton.WriteLine("Please reenter the password.");

        return this.ReadPasswordFromConsoleLine();
    }

    protected override void UserEnteredNonMatchingPasswords()
    {
        Logger.Singleton.WriteLine("Passwords do not match!");
    }

    protected override void EncryptionVerificationProcessSuccess()
    {
        Logger.Singleton.WriteLine("Encryption verification process successful.");
    }

    protected override void EncryptionProcessCompleted()
    {
        Logger.Singleton.WriteLine("Encryption and secure erase process successfully completed.");
    }

    protected override string AskUserToEnterPasswordForDecryption(string path)
    {
        Logger.Singleton.WriteLine("'" + path + "' will be decrypted. Please enter the password originally used to encrypt with.");

        return this.ReadPasswordFromConsoleLine();
    }

    protected override void DecryptionProcessCompleted()
    {
        Logger.Singleton.WriteLine("Decryption process successfully completed.");
    }

    protected override bool AskUserForEraseConfirmation(string path)
    {
        Logger.Singleton.WriteLine("Are you sure you want to start erase of: '" + path + "' (Y/N)?");
        string response = Console.ReadLine();
        if (response != "Y" && response != "y")
        {
            return false;
        }

        return true;
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