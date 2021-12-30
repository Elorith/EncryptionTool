using System;
using System.IO;

public class SecureEraser
{
    public void ErasePath(string path, SanitisationAlgorithmType type)
    {
        Logger.Singleton.WriteLine("Starting secure erase of '" + path + "'");
        
        FileAttributes attributes = File.GetAttributes(path);
        if ((attributes & FileAttributes.Directory) == FileAttributes.Directory)
        {
            foreach (string subPath in Directory.GetFileSystemEntries(path))
            {
                this.ErasePath(subPath, type, false);
            }
            Directory.Delete(path);
        }
        else
        {
        }

        Logger.Singleton.WriteLine("Secure erase complete '" + path + "'");
    }

    public void EraseFile(string path, SanitisationAlgorithmType type)
    {
        File.Delete(path);
    }

    private void EraseFast(Stream stream)
    {
    }
    
    private void EraseSensitive(Stream stream)
    {
    }
    
    private void Pass(Stream stream, bool usePseudoRandom, byte? valueToWrite = null)
    {
        stream.Position = 0;
        
        long bytes = stream.Length;
        if (this.currentBufferSize >= bytes)
        {
            return;
        }

        this.buffer = new byte[bytes];
        this.currentBufferSize = bytes;

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
                this.buffer[index] = (byte)(random.Next() % 256);;
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