using System;
using System.Windows.Forms;

public class WinformsEncryptionTool : EncryptionTool
{
    private FormMainInterface formMainInterface;
    private FormEncryptionTask formEncryptionTask;
    
    public override void RunTool()
    {
        this.formMainInterface = new FormMainInterface();

        this.formMainInterface.OnBeginEncrypt += this.BeginEncrypt;
        this.formMainInterface.OnBeginDecrypt += this.BeginDecrypt;
        
        Application.Run(this.formMainInterface);
    }

    private void BeginEncrypt(string path)
    {
        this.formEncryptionTask = new FormEncryptionTask();
        this.formEncryptionTask.Show();
        
        if (this.formMainInterface.IsEncryptionModeFiles())
        {
            this.DoFileEncryption(path);
        }
        else if (this.formMainInterface.IsEncryptionModeDirectories())
        {
            this.DoDirectoryEncryption(path);
        }
        else
        {
            throw new Exception("Neither encryption mode is selected");
        }
        
        this.formEncryptionTask.Close();
        this.formEncryptionTask = null;
    }
    
    private void BeginDecrypt(string path)
    {
        if (this.formMainInterface.IsEncryptionModeFiles())
        {
            this.DoFileDecryption(path);
        }
        else if (this.formMainInterface.IsEncryptionModeDirectories())
        {
            this.DoDirectoryDecryption(path);
        }
        else
        {
            throw new Exception("Neither encryption mode is selected");
        }
    }
    
    protected override string AskUserToEnterPasswordForEncryption(string path)
    {
        throw new System.NotImplementedException();
    }

    protected override string AskUserToRepeatPasswordForEncryption(string path)
    {
        throw new System.NotImplementedException();
    }

    protected override void UserEnteredNonMatchingPasswords()
    {
        throw new System.NotImplementedException();
    }

    protected override void EncryptionVerificationProcessSuccess()
    {
        throw new System.NotImplementedException();
    }

    protected override void EncryptionProcessCompleted()
    {
        throw new System.NotImplementedException();
    }

    protected override string AskUserToEnterPasswordForDecryption(string path)
    {
        throw new System.NotImplementedException();
    }

    protected override void DecryptionProcessCompleted()
    {
        throw new System.NotImplementedException();
    }

    protected override bool AskUserForEraseConfirmation(string path)
    {
        throw new System.NotImplementedException();
    }
}