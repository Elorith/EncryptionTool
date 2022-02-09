using System;

public class ConsoleLineLogger : ILogger
{
    public void Write(string output)
    {
        Console.Write(output);
    }
}