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

    private void AppendTimestamp()
    {
        DateTime now = DateTime.Now;
        this.builder.Append(now.Year).Append('-');
        this.builder.Append(now.Month).Append('-');
        this.builder.Append(now.Day).Append(' ');
        this.builder.Append(now.Day).Append(':');
        this.builder.Append(now.Day).Append(':');
        this.builder.Append(now.Day).Append('.');
        this.builder.Append(now.Millisecond);
    }
    
    private void AppendSeparator()
    {
        this.builder.Append(' ');
        this.builder.Append("-> ");
    }

    private void AppendCarriageReturn()
    {
        this.builder.Append('\n');
    }
}