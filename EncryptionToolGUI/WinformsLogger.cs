using System.Windows.Forms;

public class WinformsLogger : ILogger
{
    public void Write(string output)
    {
        //MessageBox.Show(output);
    }
}