using System.Windows.Forms;

public class WinformsLogger : ILogger
{
    public void Write(string output)
    {
#if DEBUG
        MessageBox.Show(output);
#endif
    }
}