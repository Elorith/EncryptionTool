using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;

public class ConsoleLineApplication
{
    private EncryptionTool tool;
    
    public EncryptionTool CreateCommandLineInterfaceTool()
    {
        this.tool = new EncryptionTool();

        this.tool.OnAskUserForEraseConfirmation += new EncryptionTool.OnAskUserForEraseConfirmationCallback((path) =>
        {
            Logger.Singleton.WriteLine("Are you sure you want to start erase of: '" + path + "' (Y/N)?");
            string response = Console.ReadLine();
            if (response != "Y" && response != "y")
            {
                return false;
            }

            return true;
        });

        this.tool.OnAskUserToEnterPasswordForEncryption += new EncryptionTool.OnAskUserToEnterPasswordForEncryptionCallback((path) =>
        {
            Logger.Singleton.WriteLine("'" + path + "' will be encrypted and securely erased. Please enter a password to encrypt with.");

            return this.ReadPasswordFromConsoleLine();
        });

        this.tool.OnAskUserToRepeatPasswordForEncryption += new EncryptionTool.OnAskUserToRepeatPasswordForEncryptionCallback((path) =>
        {
            Logger.Singleton.WriteLine("Please re-enter the password.");

            return this.ReadPasswordFromConsoleLine();
        });

        this.tool.OnUserEnteredNonMatchingPasswords += new EncryptionTool.OnUserEnteredNonMatchingPasswordsCallback(() =>
        {
            Logger.Singleton.WriteLine("Passwords do not match!");
        });

        this.tool.OnEncryptionVerificationProcessSuccess += new EncryptionTool.OnEncryptionVerificationProcessSuccessCallback(() =>
        {
            Logger.Singleton.WriteLine("Encryption verification process successful.");
        });

        this.tool.OnEncryptionProcessCompleted += new EncryptionTool.OnEncryptionProcessCompletedCallback(() =>
        {
            Logger.Singleton.WriteLine("Encryption and secure erase process successfully completed.");
        });

        this.tool.OnAskUserToEnterPasswordForDecryption += new EncryptionTool.OnAskUserToEnterPasswordForDecryptionCallback((path) =>
        {
            Logger.Singleton.WriteLine("'" + path + "' will be decrypted. Please enter the password originally used to encrypt with.");

            return this.ReadPasswordFromConsoleLine();
        });

        this.tool.OnDecryptionProcessCompleted += new EncryptionTool.OnDecryptionProcessCompletedCallback(() =>
        {
            Logger.Singleton.WriteLine("Decryption process successfully completed.");
        });

        return this.tool;
    }
    
    public void RunCommandLineInterfaceTool(out bool exitFlag)
    {
        string[] command = this.ReadCommandFromConsoleLine();
            
        if (string.Equals(command[0], "exit", StringComparison.OrdinalIgnoreCase))
        {
            exitFlag = true;
        }
            
        exitFlag = false;
        try
        {
            string path = command[1].Trim('"');
            bool isDirectory = (File.GetAttributes(path) & FileAttributes.Directory) == FileAttributes.Directory;
                
            if (string.Equals(command[0], "encrypt", StringComparison.OrdinalIgnoreCase))
            {
                if (!isDirectory)
                {
                    tool.DoFileEncryption(path);
                }
                else
                {
                    tool.DoDirectoryEncryption(path);
                }
            }
            else if (string.Equals(command[0], "decrypt", StringComparison.OrdinalIgnoreCase))
            {
                if (!isDirectory)
                {
                    tool.DoFileDecryption(path);
                }
                else
                {
                    tool.DoDirectoryDecryption(path);
                }
            }
            else if (string.Equals(command[0], "erase", StringComparison.OrdinalIgnoreCase))
            {
                tool.DoSecureErase(path, SanitisationAlgorithmType.DoDSensitive, true);
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