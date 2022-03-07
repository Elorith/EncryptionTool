using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;

public partial class FormMainInterface : Form
{
    public delegate void OnBeginEncryptCallback(string path);
    public delegate void OnBeginDecryptCallback(string path);
    
    public event OnBeginEncryptCallback OnBeginEncrypt;
    public event OnBeginDecryptCallback OnBeginDecrypt;

    private Stack<string> pathsToNavigateBackTo = new Stack<string>();
    private Stack<string> pathsToNavigateForwardTo = new Stack<string>();
    private bool checkboxChangedFlag = false;

    public FormMainInterface()
    {
        this.InitializeComponent();
    }

    public string SelectedPath
    {
        get
        {
            return this.TextBoxExplorerPath.Text;
        }

        set
        {
            if (this.TextBoxExplorerPath.Text == value)
            {
                return;
            }
            
            if (this.pathsToNavigateBackTo.Count > 0 && this.pathsToNavigateBackTo.Peek() == value)
            {
                this.pathsToNavigateBackTo.Pop();
                
                this.pathsToNavigateForwardTo.Push(this.TextBoxExplorerPath.Text);
            }
            else
            {
                this.pathsToNavigateBackTo.Push(this.TextBoxExplorerPath.Text);
            }

            if (this.pathsToNavigateForwardTo.Count > 0 && this.pathsToNavigateForwardTo.Peek() == value)
            {
                this.pathsToNavigateForwardTo.Pop();
            }

            this.TextBoxExplorerPath.Text = value;
        }
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
        
        if (result != DialogResult.Cancel && !File.Exists(path) && !Directory.Exists(path))
        {
            throw new Exception("Selected path invalid");
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
    
    public void ClearSelectedPath()
    {
        this.SelectedPath = string.Empty;
        
        this.pathsToNavigateBackTo.Clear();
        this.pathsToNavigateForwardTo.Clear();
    }
    
    private void ButtonSelectPath_Click(object sender, EventArgs e)
    {
        this.SelectedPath = this.Browse();
    }

    private void CheckBoxEncryptFiles_CheckedChanged(object sender, EventArgs e)
    {
        if (this.CheckBoxEncryptFiles.Checked)
        {
            this.checkboxChangedFlag = true;
            this.CheckBoxEncryptDirectories.Checked = false;
            this.checkboxChangedFlag = false;
            this.ClearSelectedPath();
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
            this.ClearSelectedPath();
        }
        else if (!checkboxChangedFlag)
        {
            this.CheckBoxEncryptDirectories.Checked = true;
        }
    }

    private void ButtonEncrypt_Click(object sender, EventArgs e)
    {
        this.Encrypt(this.SelectedPath);
    }

    private void ButtonDecrypt_Click(object sender, EventArgs e)
    {
        this.Decrypt(this.SelectedPath);
    }

    private void ButtonSelectedPathBack_Click(object sender, EventArgs e)
    {
        this.SelectedPath = this.pathsToNavigateBackTo.Peek();
    }

    private void ButtonSelectedPathForward_Click(object sender, EventArgs e)
    {
        this.SelectedPath = this.pathsToNavigateForwardTo.Peek();
    }
}