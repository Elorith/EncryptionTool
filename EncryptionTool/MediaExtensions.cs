using System;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;

public static class MediaExtensions
{
    private const double mediaPreviewImagePosition = 0.5;
    
    private const string ffprobeFileName = "ffprobe.exe";
    private const string ffmpegFileName = "ffmpeg.exe";

    private static readonly string[] videoExtensionsList =
    {
        "mp4", "mov", "wmv"
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
    
    public static MediaPreview LoadMediaPreview(string image)
    {
        byte[] bytes = Utilities.HexadecimalToBuffer(image);

        Stream stream = new MemoryStream(bytes);
        MediaPreview preview = new MediaPreview
        {
            Thumbnail = Image.FromStream(stream),
            UnderlyingStream = stream
        };

        return preview;
    }

    public static string GetMediaPreview(string path)
    {
        byte[] bytes = MediaExtensions.CreatePreviewImage(path, MediaExtensions.mediaPreviewImagePosition);
        if (bytes == null)
        {
            throw new FormatException("Failed to get media preview image");
        }
        
        string image = Utilities.BufferToHexadecimal(bytes);

        return image;
    }

    private static byte[] CreatePreviewImage(string path, double position)
    {
        int lengthInMilliseconds = 0;
        using (Process process1 = new Process())
        {
            process1.StartInfo = MediaExtensions.GetProcessStartInfo(Path.Combine(Environment.CurrentDirectory, MediaExtensions.ffprobeFileName), "-v error -show_entries format=duration -of default=noprint_wrappers=1:nokey=1 " + path);

            process1.Start();
            process1.WaitForExit();

            string stdout = process1.StandardOutput.ReadToEnd();
            lengthInMilliseconds = (int)(Convert.ToDouble(stdout) * 1000);
        }
        
        double positionInMilliseconds = lengthInMilliseconds * position;
        TimeSpan timespan = TimeSpan.FromMilliseconds(positionInMilliseconds);

        string seek = timespan.Hours.ToString("00") + ":" + timespan.Minutes.ToString("00") + ":" + timespan.Seconds.ToString("00") + "." + timespan.Milliseconds.ToString("000");

        string tempFileName = "ThumbTemp.jpg";
        string tempOutputPath = Path.Combine(Environment.CurrentDirectory, tempFileName);
        
        using (Process process2 = new Process())
        {
            process2.StartInfo = MediaExtensions.GetProcessStartInfo(Path.Combine(Environment.CurrentDirectory, MediaExtensions.ffmpegFileName), "-i " + path + " -ss " + seek + " -vframes 1 " + tempFileName);

            process2.Start();
            process2.WaitForExit();
        }
        
        byte[] bytes = null;
        using (MemoryStream stream = new MemoryStream())
        {
            using (Image image = Image.FromFile(tempOutputPath))
            {
                image.Save(stream, ImageFormat.Jpeg);
                bytes = stream.ToArray();
            }
        }
        
        SecureEraser eraser = new SecureEraser();
        eraser.ErasePath(tempOutputPath, SanitisationAlgorithmType.DoDSensitive);

        return bytes;
    }

    private static ProcessStartInfo GetProcessStartInfo(string processPath, string arguments)
    {
        ProcessStartInfo info = new ProcessStartInfo
        {
            FileName = processPath,
            Arguments = arguments,
            CreateNoWindow = true,
            UseShellExecute = false,
            RedirectStandardOutput = true,
            WorkingDirectory = Environment.CurrentDirectory
        };

        return info;
    }
    
    public class MediaPreview
    {
        public Image Thumbnail;
        public Stream UnderlyingStream;

        public void Dispose()
        {
            this.Thumbnail.Dispose();
            this.UnderlyingStream.Dispose();
        }
    }
}