using System;
using System.IO;
using System.Security.Authentication;
using System.Security.Cryptography;

public abstract class EncryptionTool
{
    public delegate string OnAskUserToEnterPasswordForEncryptionCallback(string path);
    public delegate string OnAskUserToRepeatPasswordForEncryptionCallback(string path);
    public delegate void OnUserEnteredNonMatchingPasswordsCallback();
    public delegate void OnEncryptionVerificationProcessSuccessCallback();
    public delegate void OnEncryptionProcessCompletedCallback();
    public delegate string OnAskUserToEnterPasswordForDecryptionCallback(string path);
    public delegate void OnDecryptionProcessCompletedCallback();
    public delegate bool OnAskUserForEraseConfirmationCallback(string path);
    
    public event OnAskUserToEnterPasswordForEncryptionCallback OnAskUserToEnterPasswordForEncryption;
    public event OnAskUserToRepeatPasswordForEncryptionCallback OnAskUserToRepeatPasswordForEncryption;
    public event OnUserEnteredNonMatchingPasswordsCallback OnUserEnteredNonMatchingPasswords;
    public event OnEncryptionVerificationProcessSuccessCallback OnEncryptionVerificationProcessSuccess;
    public event OnEncryptionProcessCompletedCallback OnEncryptionProcessCompleted;
    public event OnAskUserToEnterPasswordForDecryptionCallback OnAskUserToEnterPasswordForDecryption;
    public event OnDecryptionProcessCompletedCallback OnDecryptionProcessCompleted;
    public event OnAskUserForEraseConfirmationCallback OnAskUserForEraseConfirmation;

    public void DoFileEncryption(string path)
    {
        if (!File.Exists(path))
        {
            throw new ArgumentException("Specified path is not a file or does not exist");
        }

        string personalKey = this.AskUserForPersonalKeyForEncryption(path);
        if (personalKey == null)
        {
            return;
        }
        
        this.EncryptFile(path, personalKey, true);
    }

    public void DoDirectoryEncryption(string path)
    {
        if (!Directory.Exists(path))
        {
            throw new ArgumentException("Specified path is not a directory or does not exist");
        }
        DirectoryInfo rootParent = Directory.GetParent(path);
        if (rootParent == null)
        {
            throw new ArgumentException("Specified path has no parent");
        }

        string personalKey = this.AskUserForPersonalKeyForEncryption(path);
        if (personalKey == null)
        {
            return;
        }
        
        this.EncryptPathRecursive(path, personalKey, rootParent, true);
    }
    
    public void DoFileDecryption(string path)
    {
        if (!File.Exists(path))
        {
            throw new ArgumentException("Specified path is not a file or does not exist");
        }

        string personalKey = this.AskUserForPersonalKeyForDecryption(path);
        
        this.DecryptFile(path, personalKey, true);
    }
    
    public void DoDirectoryDecryption(string path)
    {
        if (!Directory.Exists(path))
        {
            throw new ArgumentException("Specified path is not a directory or does not exist");
        }
        DirectoryInfo rootParent = Directory.GetParent(path);
        if (rootParent == null)
        {
            throw new ArgumentException("Specified path has no parent");
        }
        
        string personalKey = this.AskUserForPersonalKeyForDecryption(path);

        this.DecryptPathRecursive(path, personalKey, rootParent, true);
    }
    
    public void DoSecureErase(string path, SanitisationAlgorithmType sanitisationType, bool askForConfirmation = true)
    {
        if (askForConfirmation)
        {
            bool result = this.OnAskUserForEraseConfirmation(path);
            if (!result)
            {
                return;
            }
        }

        SecureEraser eraser = new SecureEraser();
        eraser.ErasePath(path, sanitisationType);
    }

    private void EncryptPathRecursive(string path, string personalKey, DirectoryInfo parent, bool releasePersonalKey = true, SanitisationAlgorithmType sanitisationType = SanitisationAlgorithmType.DoDSensitive)
    {
        if ((File.GetAttributes(path) & FileAttributes.Directory) == FileAttributes.Directory)
        {
            CryptographyProvider cryptography = new CryptographyProvider();
            string outputPath = cryptography.EncryptDirectoryRootToDiskWithPersonalKey(path, personalKey, parent);
            
            Logger.Singleton.WriteLine("'" + path + "' has been successfully encrypted to disk.");
            
            DirectoryInfo outputDirectory = new DirectoryInfo(outputPath);

            foreach (string subPath in Directory.GetFileSystemEntries(path))
            {
                this.EncryptPathRecursive(subPath, personalKey, outputDirectory, false, sanitisationType);
            }
            
            MemoryManagement.HandleSensitiveResource(personalKey, personalKey.Length, releasePersonalKey);

            Directory.Delete(path);
        }
        else
        {
            string newPath = Path.Combine(parent.FullName, Path.GetFileName(path));
            File.Move(path, newPath);

            this.EncryptFile(newPath, personalKey, false, sanitisationType);
        }
    }

    private void DecryptPathRecursive(string path, string personalKey, DirectoryInfo parent, bool releasePersonalKey = true)
    {
        if ((File.GetAttributes(path) & FileAttributes.Directory) == FileAttributes.Directory)
        {
            DirectoryInfo currentDirectory = new DirectoryInfo(path);
            string headerInputPath = Path.Combine(path, "_" + currentDirectory.Name + ".aes");
            
            CryptographyProvider cryptography = new CryptographyProvider();
            string outputPath = cryptography.DecryptDirectoryRootToDiskWithPersonalKey(headerInputPath, personalKey, parent);
            
            Logger.Singleton.WriteLine("'" + path + "' has been successfully decrypted to disk.");

            DirectoryInfo outputDirectory = new DirectoryInfo(outputPath);
            
            File.Delete(headerInputPath);
            
            foreach (string subPath in Directory.GetFileSystemEntries(path))
            {
                this.DecryptPathRecursive(subPath, personalKey, outputDirectory, false);
            }
            
            MemoryManagement.HandleSensitiveResource(personalKey, personalKey.Length, releasePersonalKey);

            Directory.Delete(path);
        }
        else
        {
            string newPath = Path.Combine(parent.FullName, Path.GetFileName(path));
            File.Move(path, newPath);

            this.DecryptFile(newPath, personalKey, false);
        }
    }

    private void EncryptFile(string path, string personalKey, bool releasePersonalKey = true, SanitisationAlgorithmType sanitisationType = SanitisationAlgorithmType.DoDSensitive)
    {
        CryptographyProvider cryptography = new CryptographyProvider();
        string outputPath = cryptography.EncryptFileToDiskWithPersonalKey(path, personalKey);
        
        Logger.Singleton.WriteLine("'" + path + "' has been successfully encrypted to disk.");
        
        try
        {
            byte[] decrypted = cryptography.DecryptFileToMemoryWithPersonalKey(outputPath, personalKey);

            string hash = cryptography.HashBufferToString(decrypted, HashAlgorithmType.Md5, false);

            MemoryManagement.HandleSensitiveResource(decrypted, decrypted.Length, true);

            if (hash != Path.GetFileNameWithoutExtension(outputPath))
            {
                throw new CryptographicException("Encryption verification process failed");
            }
        }
        catch (CryptographicException ex)
        {
            throw new CryptographicException("Encryption verification process failed");
        }
        
        this.OnEncryptionVerificationProcessSuccess();
        
        MemoryManagement.HandleSensitiveResource(personalKey, personalKey.Length * 2, releasePersonalKey);
        
        this.DoSecureErase(path, sanitisationType, false);
        
        this.OnEncryptionProcessCompleted();
    }

    private void DecryptFile(string path, string personalKey, bool releasePersonalKey = true)
    {
        CryptographyProvider cryptography = new CryptographyProvider();
        cryptography.DecryptFileToDiskWithPersonalKey(path, personalKey);
        
        Logger.Singleton.WriteLine("'" + path + "' has been successfully decrypted to disk.");
        
        MemoryManagement.HandleSensitiveResource(personalKey, personalKey.Length * 2, releasePersonalKey);
        
        File.Delete(path);

        this.OnDecryptionProcessCompleted();
    }

    private string AskUserForPersonalKeyForEncryption(string path)
    {
        string response = this.OnAskUserToEnterPasswordForEncryption(path);
        string response2 = this.OnAskUserToRepeatPasswordForEncryption(path);

        string personalKey = null;
        if (response != response2)
        {
            MemoryManagement.HandleSensitiveResource(response, response.Length * 2, true);

            this.OnUserEnteredNonMatchingPasswords();
        }
        else
        {
            personalKey = response;
        }
        
        MemoryManagement.HandleSensitiveResource(response2, response2.Length * 2, true);

        return personalKey;
    }

    private string AskUserForPersonalKeyForDecryption(string path)
    {
        return this.OnAskUserToEnterPasswordForDecryption(path);
    }
    
    protected abstract string AskUserToEnterPasswordForEncryption(string path);
    protected abstract string AskUserToRepeatPasswordForEncryption(string path);
    protected abstract void UserEnteredNonMatchingPasswords();
    protected abstract void EncryptionVerificationProcessSuccess();
    protected abstract void EncryptionProcessCompleted();
    protected abstract string AskUserToEnterPasswordForDecryption(string path);
    protected abstract void DecryptionProcessCompleted();
    protected abstract bool AskUserForEraseConfirmation(string path);

    public virtual void RunTool()
    {
    }

    public EncryptionTool()
    {
        this.OnAskUserToEnterPasswordForEncryption += this.AskUserToEnterPasswordForEncryption;
        this.OnAskUserToRepeatPasswordForEncryption += this.AskUserToRepeatPasswordForEncryption;
        this.OnUserEnteredNonMatchingPasswords += this.UserEnteredNonMatchingPasswords;
        this.OnEncryptionVerificationProcessSuccess += this.EncryptionVerificationProcessSuccess;
        this.OnEncryptionProcessCompleted += this.EncryptionProcessCompleted;
        this.OnAskUserToEnterPasswordForDecryption += this.AskUserToEnterPasswordForDecryption;
        this.OnDecryptionProcessCompleted += this.DecryptionProcessCompleted;
        this.OnAskUserForEraseConfirmation += this.AskUserForEraseConfirmation;
    }
}