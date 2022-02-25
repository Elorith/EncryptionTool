using System.IO;

public static class MediaExtensions
{
    private const double mediaPreviewImagePosition = 0.0;
    
    private static readonly string[] videoExtensionsList =
    {
        "mp4", "mov"
    };

    public static bool IsFileVideo(string path)
    {
        string[] split = path.Split('.');
        
        foreach (string extension in MediaExtensions.videoExtensionsList)
        {
            if (extension == split[split.Length - 1])
            {
                return true;
            }
        }

        return false;
    }

    public static string GetMediaPreview(string path)
    {
        byte[] image = MediaExtensions.CreatePreviewImage(path, MediaExtensions.mediaPreviewImagePosition);

        return "";
    }

    private static byte[] CreatePreviewImage(string path, double position)
    {
        return null;
    }
}