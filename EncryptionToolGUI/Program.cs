using System;
using System.Windows.Forms;

public class Program
{
    /// <summary>
    /// The main entry point for the application.
    /// </summary>
    [STAThread]
    public static void Main() => new Program().RunGraphicalUserInterface();

    // GUI implementation of the encryption tool.
    public void RunGraphicalUserInterface()
    {
        MainInterfaceForm form = new MainInterfaceForm();
        
        Application.EnableVisualStyles();
        Application.SetCompatibleTextRenderingDefault(false);
        Application.Run(form);
    }
}