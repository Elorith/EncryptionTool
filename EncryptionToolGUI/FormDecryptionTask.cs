using System;
using System.Windows.Forms;

public partial class FormDecryptionTask : Form
{
    public delegate void OnFormSubmittedCallback();

    public event OnFormSubmittedCallback OnFormSubmitted;

    public FormDecryptionTask()
    {
        this.InitializeComponent();
        
        this.TextBoxPassword.PasswordChar = '*';
        this.CenterToScreen();
    }

    public string GetPasswordField()
    {
        return this.TextBoxPassword.Text;
    }

    private void ButtonSubmitTask_Click(object sender, EventArgs e)
    {
        this.OnFormSubmitted();
    }
}