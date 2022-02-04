using System;
using System.Windows.Forms;

public partial class FormEncryptionTask : Form
{
    public delegate void OnFormSubmittedCallback();

    public event OnFormSubmittedCallback OnFormSubmitted;

    public FormEncryptionTask()
    {
        this.InitializeComponent();
        
        this.CenterToScreen();
    }

    public string GetPasswordField()
    {
        return this.TextBoxPassword.Text;
    }
    
    public string GetConfirmPasswordField()
    {
        return this.TextBoxConfirmPassword.Text;
    }

    private void ButtonSubmitTask_Click(object sender, EventArgs e)
    {
        this.OnFormSubmitted();
    }
}