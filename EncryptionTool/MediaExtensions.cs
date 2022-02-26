using System;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Text;

public static class MediaExtensions
{
    private const double mediaPreviewImagePosition = 0.5;
    
    private const string ffprobeFileName = "ffprobe.exe";
    private const string ffmpegFileName = "ffmpeg.exe";

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
        int lengthInMilliseconds = 0;
        using (Process process1 = new Process())
        {
            ProcessStartInfo info1 = new ProcessStartInfo
            {
                FileName = Path.Combine(Environment.CurrentDirectory, MediaExtensions.ffprobeFileName),
                Arguments = "-v error -show_entries format=duration -of default=noprint_wrappers=1:nokey=1 " + path/*+ " -sexagesimal"*/,
                CreateNoWindow = true,
                UseShellExecute = false,
                RedirectStandardOutput = true,
                WorkingDirectory = Environment.CurrentDirectory
            };

            process1.StartInfo = info1;

            process1.Start();
            process1.WaitForExit();
            
            lengthInMilliseconds = (int)(Convert.ToDouble(process1.StandardOutput.ReadToEnd()) * 1000);
        }
        
        double positionInMilliseconds = lengthInMilliseconds * position;
        TimeSpan timespan = TimeSpan.FromMilliseconds(positionInMilliseconds);

        string seek = timespan.Hours.ToString("00") + ":" + timespan.Minutes.ToString("00") + ":" + timespan.Seconds.ToString("00") + "." + timespan.Milliseconds.ToString("000");
        
        using (Process process2 = new Process())
        {
            ProcessStartInfo info2 = new ProcessStartInfo
            {
                FileName = Path.Combine(Environment.CurrentDirectory, MediaExtensions.ffmpegFileName),
                Arguments = "-i " + path + " -ss " + seek + " -vframes 1 tempThumb.jpg",
                CreateNoWindow = true,
                UseShellExecute = false,
                WorkingDirectory = Environment.CurrentDirectory
            };

            process2.StartInfo = info2;

            process2.Start();
            process2.WaitForExit();
        }
        
        byte[] bytes = null;
        using (MemoryStream stream = new MemoryStream())
        {
            using (Image image = Image.FromFile(Path.Combine(Environment.CurrentDirectory, "tempThumb.jpg")))
            {
                image.Save(stream, ImageFormat.Jpeg);
                bytes = stream.ToArray();
            }
        }
        
        File.Delete(Path.Combine(Environment.CurrentDirectory, "tempThumb.jpg"));
        
        return bytes;
    }
}