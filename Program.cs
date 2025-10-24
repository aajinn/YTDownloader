using System;
using System.Diagnostics;
using System.IO;

namespace YTDownloader
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("=== YouTube Downloader CLI (.NET) ===");

            Console.Write("Enter YouTube URL: ");
            string? urlInput = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(urlInput))
            {
                Console.WriteLine("Invalid URL.");
                return;
            }
            string url = urlInput.Trim();

            Console.WriteLine("Select Format:");
            Console.WriteLine("1. MP4 (Video)");
            Console.WriteLine("2. MP3 (Audio)");
            Console.Write("Choice: ");
            string? choice = Console.ReadLine();
            string format = choice == "2" ? "mp3" : "mp4";

            Console.Write("Enter output directory (leave blank for current): ");
            string? outputDirInput = Console.ReadLine();
            string outputDir = string.IsNullOrWhiteSpace(outputDirInput)
                ? Directory.GetCurrentDirectory()
                : outputDirInput.Trim();

            if (!Directory.Exists(outputDir))
                Directory.CreateDirectory(outputDir);

            string ytDlpPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "yt-dlp.exe");
            if (!File.Exists(ytDlpPath))
            {
                Console.WriteLine("yt-dlp.exe not found in project folder.");
                Console.WriteLine("→ Download it from: https://github.com/yt-dlp/yt-dlp/releases");
                Console.WriteLine("→ Then place yt-dlp.exe next to your Program.exe file.");
                return;
            }

            string outputTemplate = Path.Combine(outputDir, "%(title)s.%(ext)s");
            string argsString = format == "mp3"
                ? $"-x --audio-format mp3 -o \"{outputTemplate}\" \"{url}\""
                : $"-f best -o \"{outputTemplate}\" \"{url}\"";

            try
            {
                Console.WriteLine($"\nDownloading {format.ToUpper()}...");
                var psi = new ProcessStartInfo
                {
                    FileName = ytDlpPath,
                    Arguments = argsString,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                };

                using var process = new Process { StartInfo = psi };
                process.OutputDataReceived += (s, e) => { if (e.Data != null) Console.WriteLine(e.Data); };
                process.ErrorDataReceived += (s, e) => { if (e.Data != null) Console.WriteLine(e.Data); };

                process.Start();
                process.BeginOutputReadLine();
                process.BeginErrorReadLine();
                process.WaitForExit();

                Console.WriteLine($"\nDownload completed. Saved in: {outputDir}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
        }
    }
}
