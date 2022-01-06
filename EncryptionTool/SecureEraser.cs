using System;
using System.IO;

public class SecureEraser
{
    private const long minimumBufferSize = 65536;
    private byte[] buffer;
    private long currentBufferSize;
    
    public void ErasePath(string path, SanitisationAlgorithmType sanitisationType)
    {
        FileAttributes attributes = File.GetAttributes(path);
        if ((attributes & FileAttributes.Directory) == FileAttributes.Directory)
        {
            foreach (string subPath in Directory.GetFileSystemEntries(path))
            {
                this.ErasePath(subPath, sanitisationType);
            }
            Directory.Delete(path);
        }
        else
        {
            this.EraseFile(path, sanitisationType);
        }
    }

    public void EraseFile(string path, SanitisationAlgorithmType sanitisationType)
    {
        Logger.Singleton.WriteLine("Starting secure erase of file at '" + path + "'.");
        
        using (FileStream stream = new FileStream(path, FileMode.OpenOrCreate, FileAccess.Write, FileShare.None, 4096))
        {
            if (sanitisationType == SanitisationAlgorithmType.DoDFast)
            {
                this.EraseFast(stream);
            }
            else if (sanitisationType == SanitisationAlgorithmType.DoDSensitive)
            {
                this.EraseSensitive(stream);
            }
            else
            {
                throw new ArgumentException("Specified sanitisation algorithm type is invalid");
            }
        }
        File.Delete(path);
        
        Logger.Singleton.WriteLine("Securely erased '" + path + "'.");
    }

    private void EraseFast(Stream stream)
    {
        this.Pass(stream, false, 0x00);
        this.Pass(stream, false, 0xFF);
        this.Pass(stream, true);
    }
    
    private void EraseSensitive(Stream stream)
    {
        this.EraseFast(stream);
        this.Pass(stream, true);
        this.EraseFast(stream);
    }

    private void Pass(Stream stream, bool usePseudoRandom, byte? valueToWrite = null)
    {
        stream.Position = 0;

        long bytes = stream.Length;
        long bufferSizeRequired = bytes;
        if (bufferSizeRequired < SecureEraser.minimumBufferSize)
        {
            bufferSizeRequired = SecureEraser.minimumBufferSize;
        }
        if (this.currentBufferSize < bufferSizeRequired)
        {
            this.buffer = new byte[bufferSizeRequired];
            this.currentBufferSize = bufferSizeRequired;
        }

        if (!usePseudoRandom && valueToWrite.HasValue)
        {
            for (int index = 0; index < bytes; index++)
            {
                this.buffer[index] = valueToWrite.Value;
            }
        }
        else if (usePseudoRandom && !valueToWrite.HasValue)
        {
            Random random = new Random();
            for (int index = 0; index < bytes; index++)
            {
                this.buffer[index] = (byte)(random.Next() % 256);
            }
        }
        else
        {
            throw new ArgumentException("usePseudoRandom boolean should not be true while valueToWrite is null or vice versa");
        }

        stream.Write(this.buffer, 0, this.buffer.Length);
        stream.Flush();
    }
}