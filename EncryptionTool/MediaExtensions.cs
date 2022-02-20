using System.IO;

public static class MediaExtensions
{
    private static readonly string[] videoExtensionsList =
    {
        ".mp4", ".mov"
    };

    public static bool IsPathVideo(string path)
    {
        if ((File.GetAttributes(path) & FileAttributes.Directory) == FileAttributes.Directory)
        {
            return false;
        }

        foreach (string extension in MediaExtensions.videoExtensionsList)
        {
            if (Path.HasExtension(extension))
            {
                return true;
            }
        }

        return false;
    }

    public static string GetMediaPreview(string path)
    {
        return "test";
    }
}