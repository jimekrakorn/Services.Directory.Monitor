using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using BenefitsNG.Core.Infrastructure.Extensions;
using BenefitsNG.Core.Infrastructure.Repository;
using BenefitsNG.Core.Messages.Shared.Requests;
using BenefitsNG.Core.Messages.Shared.Responses;

namespace BenefitsNG.Core.Commands.ContentManagement
{
    public class TranscodingCommand : Command<StringRequest, Response>
    {
        private Dictionary<string, string> _mp4Files;
        private Dictionary<string, string> _oggFiles;
        private string _path;

        public TranscodingCommand(StringRequest request, IRepository repository)
            : base(request, repository)
        {
        }

        protected override Response ProcessRequest()
        {
            _path = Request.String;
            buildFileLists();
            processFiles();
            return new Response();
        }

        private void buildFileLists()
        {
            _oggFiles = new Dictionary<string, string>();
            _mp4Files = new Dictionary<string, string>();

            System.IO.Directory.GetFiles(_path, "*.ogg")
                                   .Select(c => new KeyValuePair<string, string>(
                                                    System.IO.Path.GetFileNameWithoutExtension(c),
                                                    c)).ToArray()
                                    .ForEach(c => _oggFiles.Add(c.Key, c.Value));

            System.IO.Directory.GetFiles(_path, "*.mp4")
                                   .Select(c => new KeyValuePair<string, string>(
                                                    System.IO.Path.GetFileNameWithoutExtension(c),
                                                    c)).ToArray()
                                    .ForEach(c => _mp4Files.Add(c.Key, c.Value));
        }

        private void processFiles()
        {
            var filesToProcess = _mp4Files.Select(mp4 => mp4.Key)
                                .Where(c => !_oggFiles.Select(ogg => ogg.Key).Contains(c))
                                .ToArray();

            filesToProcess.ForEach(c => processFile(_mp4Files[c]));
        }

        private void processFile(string fullPathToInputFile)
        {
            // ffmpeg -i bear.mp4 -acodec libvorbis -vcodec libtheora -f ogg bear.ogg
            Process process = null;

            var waitTimeBeforeKillInSeconds = ConfigurationManager.AppSettings["FFMPEGWaitTimeBeforeForcingExitInSeconds"].To<int>();

            // TODO - I really don't like this.
            if (waitTimeBeforeKillInSeconds == 0)
                waitTimeBeforeKillInSeconds = 15;

            try
            {
                process = new Process
                    {
                        StartInfo = new ProcessStartInfo()
                    };

                var dir = ConfigurationManager.AppSettings["ffmpegFolder"];
                var path = System.IO.Path.Combine(dir, "ffmpeg.exe");

                var argFormat = "-i \"{0}\" -acodec libvorbis -vcodec libtheora -f ogg \"{1}\"";

                var outFile = fullPathToInputFile.Substring(0, fullPathToInputFile.LastIndexOf(".")) + ".ogg";
                var args = string.Format(argFormat, fullPathToInputFile, outFile);

                process.StartInfo.FileName = path;
                process.StartInfo.Arguments = args;
                process.StartInfo.UseShellExecute = false;
                process.StartInfo.CreateNoWindow = true;
                process.StartInfo.RedirectStandardError = true;
                process.StartInfo.RedirectStandardOutput = false;
                process.Start();

                var standardError = process.StandardError.ReadToEnd();

                process.WaitForExit(waitTimeBeforeKillInSeconds * 1000);

                if (!System.IO.File.Exists(outFile))
                    throw new Exception(string.Format("File not transcoded: {0} {1}", fullPathToInputFile, standardError));
            }
            finally
            {
                if (process != null && !process.HasExited)
                    process.Kill();
            }
        }
    }
}