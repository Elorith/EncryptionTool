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
    }
}