using System.IO;

public static class MediaExtensions
{
    private static readonly string[] videoExtensionsList =
    {
        ".mp4", ".mov"
    };

    public static bool IsFileOrFolderVideo(string path)
    {
        foreach (string extension in MediaExtensions.videoExtensionsList)
        {
            if (Path.HasExtension(extension))
            {
                return true;
            }
        }

        return false;
    }
}