using System;
using System.IO;

public static class Program
{
    public static void Main(string[] args)
    {
        /*
         * 1) Take file and encrypt using the user's personal key (which needs to be entered each time a security related action is taken).
         * 2) Write encrypted variation of the file to disk.
         * 3) Verify that the newly written file is valid.
         * 4) Securely erase the original file from disk using implementation of US DoD 5220.22-M (ECE).
         */
        while (true)
        {
            string path = Console.ReadLine();
            Program.DoErase(path, DoDAlgorithmType.DoDSensitive, true);
        }
    }
    
    public void DoErase(string path, SanitisationAlgorithmType type, bool askForConfirmation = true)
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
        eraser.ErasePath();
    }
} 