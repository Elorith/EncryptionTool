using System;
using System.Windows.Forms;

public class WinformsEncryptionTool : EncryptionTool
{
    private FormMainInterface mainInterface;
    
    public override void RunTool()
    {
        this.mainInterface = new FormMainInterface();
        this.mainInterface.Text = @"Encryption Tool";

        this.mainInterface.OnBeginEncrypt += this.BeginEncrypt;
        this.mainInterface.OnBeginDecrypt += this.BeginDecrypt;
        
        Application.Run(this.mainInterface);
    }

    private void BeginEncrypt(string path)
    {
        if (!this.mainInterface.IsEncryptionModeFiles())
        {
            this.DoFileDecryption(path);
        }
        else if (!this.mainInterface.IsEncryptionModeDirectories())
        {
            this.DoDirectoryDecryption(path);
        }
        else
        {
            throw new Exception("Neither encryption mode is selected");
        }
    }
    
    private void BeginDecrypt(string path)
    {
        if (!this.mainInterface.IsEncryptionModeFiles())
        {
            this.DoFileDecryption(path);
        }
        else if (!this.mainInterface.IsEncryptionModeDirectories())
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