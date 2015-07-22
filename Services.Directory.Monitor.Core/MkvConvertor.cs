using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

namespace Services.Directory.Monitor.Core
{
    public class MkvConvertor
    {
        private List<string> LockedFiles { get; set; } 
        private string FilePath { get; set; }

        public string LogOutput { get; set; }

        public MkvConvertor(string filePath)
        {
            FilePath = filePath;
            LockedFiles = new List<string>();
        }

        public void Convert(string destinationPath)
        {
            if (File.Exists(destinationPath))
            {
                File.Delete(destinationPath);
            }    
            if (LockedFiles.Contains(FilePath))
            {
                return; 
            }    
            LockFile();
            
            DirectoryMonitorLog.LogToEventViewer(
                string.Format("Starting Mk4 to Mp4 Conversion - {0}", 
                destinationPath));

            var process = ExecuteConversion();

            LogOutput = process.StandardOutput.ReadToEnd();
            process.WaitForExit();

            DirectoryMonitorLog.LogToEventViewer(
                string.Format("Mk4 to Mp4 Conversion Log - {0}\n{1}",
                destinationPath,
                LogOutput));
            DirectoryMonitorLog.LogToEventViewer(
                string.Format("Mk4 to Mp4 Conversion Completed - {0}", 
                destinationPath));
        }

        private Process ExecuteConversion()
        {
            var currentDirectory = System.IO.Directory.GetCurrentDirectory();
            var ffmpeg = currentDirectory + @"\ffmpeg\ffmpeg.exe";
            var ffmpegArgs = string.Format("-i {0} -vcodec copy -acodec copy {0}.mp4", FilePath);
            var processStartInfo = new ProcessStartInfo
            {
                FileName = ffmpeg,
                Arguments = ffmpegArgs,
                CreateNoWindow = true,
                RedirectStandardError = true,
                RedirectStandardOutput = true,
                UseShellExecute = false,
                WindowStyle = ProcessWindowStyle.Normal,
                WorkingDirectory = currentDirectory
            };
            var process = Process.Start(processStartInfo);
            DirectoryMonitorLog.LogToEventViewer(
                string.Format(
                    "Command: {0} {1}",
                    ffmpeg,
                    ffmpegArgs));
            return process;
        }

        private void LockFile()
        {
            LockedFiles.Add(FilePath);
        }
    }
}