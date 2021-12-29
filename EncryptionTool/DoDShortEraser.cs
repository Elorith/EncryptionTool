using System;
using System.IO;

/// <summary>
/// Implements the US DoD 5220.22-M (E) data sanitisation algorithm. Overwrites files 3 times. This method offers "medium" security. Use it only for
/// files that do not contain highly sensitive information.
/// </summary>
public class DoDShortEraser : IEraser
{
    public void Erase(string path)
    {
        if (!File.Exists(path))
        {
            throw new FileNotFoundException("Path does not exist");
        }
        using (FileStream stream = new FileStream(path, FileMode.OpenOrCreate, FileAccess.Write, FileShare.None, 4096, FileOptions.Asynchronous))
        {
            this.Pass(stream, false,0x00);
            this.Pass(stream, false, 0x00);
            
        }
    }

    private void Pass(FileStream stream, bool useRandomValue, byte value)
    {
        stream.Position = 0;
        
        long bytes = stream.Length;
        byte[] buffer = new byte[bytes];

        if (useRandomValue)
        {
            for (int index = 0; index < bytes; index++)
            {
                buffer[index] = value;
            }
        }
        else
        {
            Random random = new Random();
            for (int index = 0; index < bytes; index++)
            {
                buffer[index] = (byte)(random.Next() % 256);;
            }
        }

        stream.Write(buffer, 0, buffer.Length);
        stream.Flush();
    }
}