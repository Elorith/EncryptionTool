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
        }
    }

    private void Pass(FileStream stream, byte value)
    {
        long bytes = stream.Length;
        for (int index = 0; index < bytes; index++)
        {
            stream.WriteByte(value);
        }
        stream.Flush();
    }
}