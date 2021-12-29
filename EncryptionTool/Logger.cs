using System;
using System.Text;
using System.Threading;

public class Logger
{
    private readonly StringBuilder builder = new StringBuilder(1024);
    [ThreadStatic]
    private static Logger singleton;
    
    public static Logger Singleton => Logger.singleton ?? (Logger.singleton = new Logger());

    public void WriteLine()
    {
    }
}