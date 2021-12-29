using System;

public static class Program
{
    public static void Main(string[] args)
    {
        string path = Console.ReadLine();

        IEraser eraser = new DoDShortEraser();
        eraser.Erase(path);
    }
} 