using System;
using System.IO;
using System.Windows.Forms;

public partial class FormMainInterface : Form
{
    private bool checkboxChangedFlag = false;
    
    public FormMainInterface()
    {
        this.InitializeComponent();
    }

    public bool HasFileEncryptionMode()
    {
        return this.CheckBoxEncryptFiles.Checked && !this.CheckBoxEncryptDirectories.Checked;
    }
    
    public bool HasDirectoryEncryptionMode()
    {
        return this.CheckBoxEncryptDirectories.Checked && !this.CheckBoxEncryptFiles.Checked;
    }

    public string Browse()
    {
        string path;
        if (this.HasFileEncryptionMode())
        {
            DialogResult result = this.DialogSelectPathFile.ShowDialog();
            path = this.DialogSelectPathFile.FileName;
            
            if (result != DialogResult.OK || !File.Exists(path))
            {
                throw new Exception("Selected file path invalid");
            }
        }
        else if (this.HasDirectoryEncryptionMode())
        {
            DialogResult result = this.DialogSelectPathDirectory.ShowDialog();
            path = this.DialogSelectPathDirectory.SelectedPath;
            
            if (result != DialogResult.OK || !Directory.Exists(path))
            {
                throw new Exception("Selected directory path invalid");
            }
        }
        else
        {
            throw new Exception("Neither encryption mode is selected");
        }
        
        return path;
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
}