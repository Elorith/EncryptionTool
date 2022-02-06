using System;
using System.IO;
using System.Windows.Forms;

public partial class FormMainInterface : Form
{
    public delegate void OnBeginEncryptCallback(string path);
    public delegate void OnBeginDecryptCallback(string path);
    
    public event OnBeginEncryptCallback OnBeginEncrypt;
    public event OnBeginDecryptCallback OnBeginDecrypt;
    
    private bool checkboxChangedFlag = false;

    public FormMainInterface()
    {
        this.InitializeComponent();
    }

    public bool IsEncryptionModeFiles()
    {
        return this.CheckBoxEncryptFiles.Checked && !this.CheckBoxEncryptDirectories.Checked;
    }

    public bool IsEncryptionModeDirectories()
    {
        return this.CheckBoxEncryptDirectories.Checked && !this.CheckBoxEncryptFiles.Checked;
    }

    public string Browse()
    {
        DialogResult result;
        string path;
        if (this.IsEncryptionModeFiles())
        { 
            result = this.DialogSelectPathFile.ShowDialog();
            path = this.DialogSelectPathFile.FileName;
        }
        else if (this.IsEncryptionModeDirectories())
        {
            result = this.DialogSelectPathDirectory.ShowDialog();
            path = this.DialogSelectPathDirectory.SelectedPath;
        }
        else
        {
            throw new Exception("Neither encryption mode is selected");
        }
        
        if (result != DialogResult.Cancel && !File.Exists(path))
        {
            throw new Exception("Selected directory path invalid");
        }

        return path;
    }

    public void Encrypt(string path)
    {
        if (!this.IsEncryptionModeFiles() && !this.IsEncryptionModeDirectories())
        {
            throw new Exception("Neither encryption mode is selected");
        }

        this.OnBeginEncrypt(path);
    }

    public void Decrypt(string path)
    {
        if (!this.IsEncryptionModeFiles() && !this.IsEncryptionModeDirectories())
        {
            throw new Exception("Neither encryption mode is selected");
        }

        this.OnBeginDecrypt(path);
    }

    private void ButtonSelectPath_Click(object sender, EventArgs e)
    {
        this.TextBoxSelectedPath.Text = this.Browse();
    } 

    private void CheckBoxEncryptFiles_CheckedChanged(object sender, EventArgs e)
    {
        if (this.CheckBoxEncryptFiles.Checked)
        {
            this.checkboxChangedFlag = true;
            this.CheckBoxEncryptDirectories.Checked = false;
            this.checkboxChangedFlag = false;
            this.TextBoxSelectedPath.Text = "";
        }
        else if (!checkboxChangedFlag)
        {
            this.CheckBoxEncryptFiles.Checked = true;
        }
    }

    private void CheckBoxEncryptDirectories_CheckedChanged(object sender, EventArgs e)
    {
        if (this.CheckBoxEncryptDirectories.Checked)
        {
            this.checkboxChangedFlag = true;
            this.CheckBoxEncryptFiles.Checked = false;
            this.checkboxChangedFlag = false;
            this.TextBoxSelectedPath.Text = "";
        }
        else if (!checkboxChangedFlag)
        {
            this.CheckBoxEncryptDirectories.Checked = true;
        }
    }

    private void ButtonEncrypt_Click(object sender, EventArgs e)
    {
        this.Encrypt(this.TextBoxSelectedPath.Text);
    }

    private void ButtonDecrypt_Click(object sender, EventArgs e)
    {
        this.Decrypt(this.TextBoxSelectedPath.Text);
    }
}