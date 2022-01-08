using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Security.Authentication;
using System.Security.Cryptography;

public class EncryptionTool
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
        
        this.EncryptFile(path, personalKey);
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

        this.EncryptPathRecursive(path, personalKey, rootParent);
    }
    
    public void DoFileDecryption(string path)
    {
        if (!File.Exists(path))
        {
            throw new ArgumentException("Specified path is not a file or does not exist");
        }

        string personalKey = this.AskUserForPersonalKeyForDecryption(path);
        
        this.DecryptFile(path, personalKey);
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

    private void EncryptPathRecursive(string path, string personalKey, DirectoryInfo parent, SanitisationAlgorithmType sanitisationType = SanitisationAlgorithmType.DoDSensitive)
    {
        if ((File.GetAttributes(path) & FileAttributes.Directory) == FileAttributes.Directory)
        {
            CryptographyProvider cryptography = new CryptographyProvider();
            string outputPath = cryptography.EncryptDirectoryRootToDiskWithPersonalKey(path, personalKey, parent);
            
            DirectoryInfo outputDirectory = new DirectoryInfo(outputPath);

            foreach (string subPath in Directory.GetFileSystemEntries(path))
            {
                this.EncryptPathRecursive(subPath, personalKey, outputDirectory, sanitisationType);
            }
            
            Directory.Delete(path);
        }
        else
        {
            string newPath = Path.Combine(parent.FullName, Path.GetFileName(path));
            File.Move(path, newPath);
            
            this.EncryptFile(newPath, personalKey, sanitisationType);
        }
    }

    private void EncryptFile(string path, string personalKey, SanitisationAlgorithmType sanitisationType = SanitisationAlgorithmType.DoDSensitive)
    {
        CryptographyProvider cryptography = new CryptographyProvider();
        string outputPath = cryptography.EncryptFileToDiskWithPersonalKey(path, personalKey);
        
        try
        {
            byte[] decrypted = cryptography.DecryptFileToMemoryWithPersonalKey(outputPath, personalKey);

            string hash = cryptography.HashBufferToString(decrypted, HashAlgorithmType.Md5, false);
            
            GCHandle handle = this.AllocatePinnedGarbageCollectionHandle(decrypted);
            this.SecurelyReleasePinnedGarbageCollectionHandle(handle, decrypted.Length);

            if (hash != Path.GetFileNameWithoutExtension(outputPath))
            {
                throw new CryptographicException("Encryption verification process failed");
            }
        }
        catch
        {
            throw new CryptographicException("Encryption verification process failed");
        }
        
        this.OnEncryptionVerificationProcessSuccess();
        
        GCHandle handle2 = this.AllocatePinnedGarbageCollectionHandle(personalKey);
        this.SecurelyReleasePinnedGarbageCollectionHandle(handle2, personalKey.Length * 2);

        this.DoSecureErase(path, sanitisationType, false);
        
        this.OnEncryptionProcessCompleted();
    }

    private void DecryptFile(string path, string personalKey)
    {
        CryptographyProvider cryptography = new CryptographyProvider();
        cryptography.DecryptFileToDiskWithPersonalKey(path, personalKey);
        
        GCHandle handle = this.AllocatePinnedGarbageCollectionHandle(personalKey);
        this.SecurelyReleasePinnedGarbageCollectionHandle(handle, personalKey.Length * 2);
        
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
            GCHandle handle = this.AllocatePinnedGarbageCollectionHandle(response);
            this.SecurelyReleasePinnedGarbageCollectionHandle(handle, response.Length * 2);
            
            this.OnUserEnteredNonMatchingPasswords();
        }
        else
        {
            personalKey = response;
        }
        
        GCHandle handle2 = this.AllocatePinnedGarbageCollectionHandle(response2);
        this.SecurelyReleasePinnedGarbageCollectionHandle(handle2, response2.Length * 2);

        return personalKey;
    }

    private string AskUserForPersonalKeyForDecryption(string path)
    {
        return this.OnAskUserToEnterPasswordForDecryption(path);
    }

    [DllImport("Kernel32.dll", EntryPoint = "RtlZeroMemory")]
    private static extern bool ZeroMemory(IntPtr destination, int length);

    private GCHandle AllocatePinnedGarbageCollectionHandle(object value)
    {
        return GCHandle.Alloc(value, GCHandleType.Pinned);
    }

    private void SecurelyReleasePinnedGarbageCollectionHandle(GCHandle handle, int length)
    {
        EncryptionTool.ZeroMemory(handle.AddrOfPinnedObject(), length);
        handle.Free();
    }
}