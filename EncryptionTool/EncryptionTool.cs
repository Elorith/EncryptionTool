using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Security.Cryptography;

public class EncryptionTool
{
    public delegate string OnAskUserToEnterPasswordForEncryptionCallback(string path);
    public delegate string OnAskUserToRepeatPasswordForEncryptionCallback(string path);
    public delegate void OnUserEnteredNonMatchingPasswordsCallback();
    public delegate void OnEncryptionVerificationProcessSuccessCallback();
    public delegate void OnEncryptionAndSecureEraseProcessCompletedCallback();
    public delegate string OnAskUserToEnterPasswordForDecryptionCallback(string path);
    public delegate void OnDecryptionProcessCompletedCallback();
    public delegate bool OnAskUserForEraseConfirmationCallback(string path);
    
    public event OnAskUserToEnterPasswordForEncryptionCallback OnAskUserToEnterPasswordForEncryption;
    public event OnAskUserToRepeatPasswordForEncryptionCallback OnAskUserToRepeatPasswordForEncryption;
    public event OnUserEnteredNonMatchingPasswordsCallback OnUserEnteredNonMatchingPasswords;
    public event OnEncryptionVerificationProcessSuccessCallback OnEncryptionVerificationProcessSuccess;
    public event OnEncryptionAndSecureEraseProcessCompletedCallback OnEncryptionAndSecureEraseProcessCompleted;
    public event OnAskUserToEnterPasswordForDecryptionCallback OnAskUserToEnterPasswordForDecryption;
    public event OnDecryptionProcessCompletedCallback OnDecryptionProcessCompleted;
    public event OnAskUserForEraseConfirmationCallback OnAskUserForEraseConfirmation;

    [DllImport("Kernel32.dll", EntryPoint = "RtlZeroMemory")]
    private static extern bool ZeroMemory(IntPtr destination, int length);

    public void DoFileEncryption(string path)
    {
        if (!File.Exists(path))
        {
            throw new ArgumentException("Specified path is not a file or does not exist");
        }
        
        string response = this.OnAskUserToEnterPasswordForEncryption(path);
        GCHandle responseHandle = GCHandle.Alloc(response, GCHandleType.Pinned);
        
        string response2 = this.OnAskUserToRepeatPasswordForEncryption(path);
        GCHandle response2Handle = GCHandle.Alloc(response2, GCHandleType.Pinned);

        if (response != response2)
        {
            this.OnUserEnteredNonMatchingPasswords();
            return;
        }
        
        EncryptionTool.ZeroMemory(response2Handle.AddrOfPinnedObject(), response2.Length * 2);
        response2Handle.Free();

        CryptographyProvider cryptography = new CryptographyProvider();
        string outputPath = cryptography.EncryptFileToDiskWithPersonalKey(path, response);
        
        try
        {
            byte[] decrypted = cryptography.DecryptFileToMemoryWithPersonalKey(outputPath, response);
            string hash = cryptography.HashBufferToString(decrypted);

            if (hash != Path.GetFileNameWithoutExtension(outputPath))
            {
                throw new CryptographicException("Encryption verification process failed");
            }
        }
        catch (Exception ex)
        {
            throw new CryptographicException("Encryption verification process failed");
        }
        this.OnEncryptionVerificationProcessSuccess();

        EncryptionTool.ZeroMemory(responseHandle.AddrOfPinnedObject(), response.Length * 2);
        responseHandle.Free();

        this.DoSecureErase(path, SanitisationAlgorithmType.DoDSensitive, false);

        this.OnEncryptionAndSecureEraseProcessCompleted();
    }
    
    public void DoFileDecryption(string path)
    {
        if (!File.Exists(path))
        {
            throw new ArgumentException("Specified path is not a file or does not exist");
        }

        string response = this.OnAskUserToEnterPasswordForDecryption(path);
        GCHandle responseHandle = GCHandle.Alloc(response, GCHandleType.Pinned); 

        CryptographyProvider cryptography = new CryptographyProvider();
        cryptography.DecryptFileToDiskWithPersonalKey(path, response);
        
        EncryptionTool.ZeroMemory(responseHandle.AddrOfPinnedObject(), response.Length * 2);
        responseHandle.Free();
        
        File.Delete(path);

        this.OnDecryptionProcessCompleted();
    }
    
    public void DoSecureErase(string path, SanitisationAlgorithmType type, bool askForConfirmation = true)
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
        eraser.ErasePath(path, type);
    }
}