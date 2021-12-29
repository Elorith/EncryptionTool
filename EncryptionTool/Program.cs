using System;

public static class Program
{
    public static void Main(string[] args)
    {
        string path = Console.ReadLine();
        Logger.Singleton.WriteLine("Starting secure erase of file at: '" + path + "'");

        DoDEraser eraser = new DoDEraser();
        eraser.Erase(path, (int)DoDAlgorithmType.DoDSensitive);
        
        Logger.Singleton.WriteLine("Secure erase complete");
    }
} 