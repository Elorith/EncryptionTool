using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;

public class DoDEraser : IEraser
{
    private const long defaultBufferSize = 65536;
    private byte[] buffer = new byte[DoDEraser.defaultBufferSize];
    private long currentBufferSize = DoDEraser.defaultBufferSize;

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

    [SuppressMessage("ReSharper.DPA", "DPA0001: Memory allocation issues")]
    private void CheckMalloc(long bytes)
    {
        if (this.currentBufferSize >= bytes)
        {
            return;
        }

        this.buffer = new byte[bytes];
        this.currentBufferSize = bytes;
    }

    private void Pass(Stream stream, bool usePseudoRandom, byte? valueToWrite = null)
    {
        stream.Position = 0;
        
        long bytes = stream.Length;
        this.CheckMalloc(bytes);

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