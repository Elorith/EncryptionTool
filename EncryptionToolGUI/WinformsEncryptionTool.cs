using System;
using System.IO;
using System.Windows.Forms;

public class WinformsEncryptionTool : EncryptionTool
{
    private FormMainInterface formMainInterface;
    private FormEncryptionTask formEncryptionTask;
    private FormDecryptionTask formDecryptionTask;
    
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
        this.formEncryptionTask.OnFormSubmitted += () => this.ContinueEncrypt(path);
        this.formEncryptionTask.FormClosed += (sender, args) => this.AfterEncrypt();
        this.formEncryptionTask.Show();
    }

    private void ContinueEncrypt(string path)
    {
        if (File.Exists(path) || Directory.Exists(path))
        {
#if !DEBUG
            try
            {
#endif
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
                MessageBox.Show("Successfully encrypted the specified path.");
#if !DEBUG
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error! " + ex.Message + ".");
            }
#endif
        }

        this.formEncryptionTask.Close();
    }

    private void AfterEncrypt()
    {
        this.formMainInterface.ClearSelectedPath();
        this.formEncryptionTask = null;
    }
    
    private void BeginDecrypt(string path)
    {
        this.formDecryptionTask = new FormDecryptionTask();
        this.formDecryptionTask.OnFormSubmitted += () => this.ContinueDecrypt(path);
        this.formDecryptionTask.FormClosed += (sender, args) => this.AfterDecrypt();
        this.formDecryptionTask.Show();
    }

    private void ContinueDecrypt(string path)
    {
        if (File.Exists(path) || Directory.Exists(path))
        {
#if !DEBUG
            try
            {
#endif
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
                MessageBox.Show("Successfully decrypted the specified path.");
#if !DEBUG
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error! " + ex.Message + ".");
            }
#endif
        }

        this.formDecryptionTask.Close();
    }
    
    private void AfterDecrypt()
    {
        this.formMainInterface.ClearSelectedPath();
        this.formDecryptionTask = null;
    }
    
    protected override string AskUserToEnterPasswordForEncryption(string path)
    {
        return this.formEncryptionTask.GetPasswordField();
    }

    protected override string AskUserToRepeatPasswordForEncryption(string path)
    {
        return this.formEncryptionTask.GetConfirmPasswordField();
    }

    protected override void UserEnteredNonMatchingPasswords()
    {
        throw new Exception("Entered passwords did not match");
    }

    protected override void EncryptionVerificationProcessSuccess()
    {
    }

    protected override void EncryptionProcessCompleted()
    {
    }

    protected override string AskUserToEnterPasswordForDecryption(string path)
    {
        return this.formDecryptionTask.GetPasswordField();
    }

    protected override void DecryptionProcessCompleted()
    {
    }

    protected override bool AskUserForEraseConfirmation(string path)
    {
        throw new System.NotImplementedException();
    }
}