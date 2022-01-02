using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Security.Cryptography;

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
        application.CommandLineInterface();
    }

    // Command-line interface implementation of the encryption tool.
    public void CommandLineInterface()
    {
        EncryptionTool tool = new EncryptionTool();

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
} 