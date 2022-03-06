using System;
using System.Text;

public class Logger
{
    private readonly StringBuilder builder = new StringBuilder(1024);
    private ILogger handle;
    [ThreadStatic]
    private static Logger singleton;

    public static Logger Singleton => Logger.singleton ?? (Logger.singleton = new Logger());
    
    public void SetHandle(ILogger logger)
    {
        this.handle = logger;
    }

    public void WriteLine(string message)
    {
        this.builder.Clear();
        
        this.AppendTimestamp();
        this.AppendSeparator();
        this.AppendMessage(message);
        this.AppendCarriageReturn(); 
        
        string text = this.builder.ToString();
        this.WriteToHandle(text);
    }

    private void WriteToHandle(string output)
    {
        this.handle?.Write(output);
    }

    private void AppendTimestamp()
    {
        DateTime now = DateTime.Now;
        this.builder.Append(now.ToString("yyyy")).Append('-');
        this.builder.Append(now.ToString("MM")).Append('-');
        this.builder.Append(now.ToString("dd")).Append(' ');
        this.builder.Append(now.ToString("HH")).Append(':');
        this.builder.Append(now.ToString("mm")).Append(':');
        this.builder.Append(now.ToString("ss")).Append('.');
        this.builder.Append(now.ToString("fff"));
    }
    
    private void AppendSeparator()
    {
        this.builder.Append(' ');
        this.builder.Append("-> ");
    }

    private void AppendMessage(string message)
    {
        this.builder.Append(message);
    }

    private void AppendCarriageReturn()
    {
        this.builder.Append('\n');
    }
}