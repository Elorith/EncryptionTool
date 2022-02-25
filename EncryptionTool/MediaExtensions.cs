using System;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Text;

public static class MediaExtensions
{
    private const double mediaPreviewImagePosition = 0.5;
    
    private const string ffmpegFileName = "ffmpeg.exe";
    private const string ffmpegPathTempOutputFile = "tempThumb.jpg";
    
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
        byte[] bytes = MediaExtensions.CreatePreviewImage(path, MediaExtensions.mediaPreviewImagePosition);
        if (bytes == null)
        {
            throw new FormatException("Failed to get media preview image");
        }
        
        string image = Encoding.UTF8.GetString(bytes);
        
        return image;
    }

    private static byte[] CreatePreviewImage(string path, double position)
    {
        ProcessStartInfo info = new ProcessStartInfo();
        info.FileName = Path.Combine(Environment.CurrentDirectory, MediaExtensions.ffmpegFileName);
        info.Arguments = "-i " + path + " -ss 00:00:00.100 -vframes 1 " + MediaExtensions.ffmpegPathTempOutputFile;
        info.CreateNoWindow = true;
        info.UseShellExecute = false;
        info.WorkingDirectory = Environment.CurrentDirectory;

        using (Process process = new Process())
        {
            process.StartInfo = info;

            process.Start();
            process.WaitForExit();
        }

        byte[] bytes;
        using (MemoryStream stream = new MemoryStream())
        {
            Image image = Image.FromFile(Path.Combine(Environment.CurrentDirectory, MediaExtensions.ffmpegPathTempOutputFile));
            
            image.Save(stream, ImageFormat.Jpeg);
            bytes = stream.ToArray();
        }
        
        File.Delete(MediaExtensions.ffmpegPathTempOutputFile);
        
        return bytes;
    }
}