using System;
using System.Text;

public static class Utilities
{
    public static string BufferToHexadecimal(byte[] buffer)
    {
        StringBuilder builder = new StringBuilder();
        for (int index = 0; index < buffer.Length; index++)
        {
            byte value = buffer[index];
            builder.Append(value.ToString("x2"));  
        }
          
        return builder.ToString();
    }
     
    public static byte[] HexadecimalToBuffer(string hex)
    {
        int length = hex.Length;
          
        byte[] buffer = new byte[length / 2];
        for (int index = 0; index < length; index += 2)
        {
            buffer[index / 2] = Convert.ToByte(hex.Substring(index, 2), 16);
        }

        return buffer;
    }
}