using System;
using System.IO;

public static class Program
{
    public static void Main(string[] args)
    {
        while (true)
        {
            string path = Console.ReadLine();
            Program.DoErasePathRecursive(path, DoDAlgorithmType.DoDSensitive, true);
        }
    }

    private static void DoErasePathRecursive(string path, DoDAlgorithmType type, bool askForConfirmation = true)
    {
        Logger.Singleton.WriteLine("Starting secure erase of '" + path + "'");

        if (askForConfirmation)
        {
            Logger.Singleton.WriteLine("Are you sure you want to start erase of: '" + path + "' (Y/N)?");
        
            string response = Console.ReadLine();
            if (response != "Y")
            {
                return;
            }   
        }
        
        FileAttributes attributes = File.GetAttributes(path);
        if ((attributes & FileAttributes.Directory) == FileAttributes.Directory)
        {
            foreach (string subPath in Directory.GetFileSystemEntries(path))
            {
                Program.DoErasePathRecursive(subPath, type, false);
            }
            Directory.Delete(path);
        }
        else
        {
            DoDEraser eraser = new DoDEraser();
            eraser.Erase(path, type);
            File.Delete(path);
        }

        Logger.Singleton.WriteLine("Secure erase complete '" + path + "'");
    }
} 