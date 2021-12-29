using System;
using System.IO;

public class DoDEraser : IEraser
{
    public void Erase(string path)
    {
        this.Erase(path, DoDAlgorithmType.DoDSensitive);
    }
    
    public void Erase(string path, int type)
    {
        this.Erase(path, (DoDAlgorithmType)type);
    }

    public void Erase(string path, DoDAlgorithmType type)
    {
        if (!File.Exists(path))
        {
            throw new FileNotFoundException("Path does not exist");
        }

        if (type == DoDAlgorithmType.DoDFast)
        {
            this.EraseFast(path);
        }
        else if (type == DoDAlgorithmType.DoDSensitive)
        {
            this.EraseSensitive(path);
        }
        else
        {
            throw new ArgumentException("Specified algorithm does not exist");
        }
    }

    private void EraseFast(string path)
    {
        using (FileStream stream = new FileStream(path, FileMode.OpenOrCreate, FileAccess.Write, FileShare.None, 4096))
        {
            this.Pass(stream, false, 0x00);
            this.Pass(stream, false, 0xFF);
            this.Pass(stream, true);
        }
    }
    
    private void EraseSensitive(string path)
    {
        using (FileStream stream = new FileStream(path, FileMode.OpenOrCreate, FileAccess.Write, FileShare.None, 4096))
        {
            this.Pass(stream, false, 0x00);
            this.Pass(stream, false, 0xFF);
            this.Pass(stream, true);

            this.Pass(stream, true);

            this.Pass(stream, false, 0x00);
            this.Pass(stream, false, 0xFF);
            this.Pass(stream, true);
        }
    }

    private void Pass(FileStream stream, bool useRandomValue, byte? valueToWrite = null)
    {
        stream.Position = 0;
        
        long bytes = stream.Length;
        byte[] buffer = new byte[bytes];

        if (!useRandomValue && valueToWrite.HasValue)
        {
            for (int index = 0; index < bytes; index++)
            {
                buffer[index] = valueToWrite.Value;
            }
        }
        else if (useRandomValue && !valueToWrite.HasValue)
        {
            Random random = new Random();
            for (int index = 0; index < bytes; index++)
            {
                buffer[index] = (byte)(random.Next() % 256);;
            }
        }
        else
        {
            throw new ArgumentException("useRandomValue boolean should not be true while valueToWrite is null or vice versa");
        }

        stream.Write(buffer, 0, buffer.Length);
        stream.Flush();
    }
}