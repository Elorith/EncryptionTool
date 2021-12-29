using System;

public static class Program
{
    public static void Main(string[] args)
    {
        string path = Console.ReadLine();
        Logger.Singleton.WriteLine("Starting secure erase of file at: '" + path + "'");

        IEraser eraser = new DoDShortEraser();
        eraser.Erase(path);
        
        Logger.Singleton.WriteLine("Secure erase complete");
    }
} 