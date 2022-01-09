using System;

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

        ConsoleLineApplication application = new ConsoleLineApplication();
        application.CreateCommandLineInterfaceTool();
        
        bool exitFlag = false;
        while (!exitFlag)
        {
            application.RunCommandLineInterfaceTool(out exitFlag);
        }
    }
} 