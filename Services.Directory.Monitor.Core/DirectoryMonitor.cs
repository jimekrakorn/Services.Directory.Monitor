using System.IO;
using System.Threading.Tasks;
using System.Timers;
using IODirectory = System.IO.Directory;
using IOFile = System.IO.File;

namespace Services.Directory.Monitor.Core
{
    public class DirectoryMonitor
    {
        private readonly Timer _timer;
        private DirectoryMonitorOptions _options;

        public DirectoryMonitor(DirectoryMonitorOptions options)
        {
            _options = options;
            _timer = new Timer
            {
                Interval = _options.DirectoryMonitorInterval,
                Enabled = true
            };
            _timer.Elapsed += TimerOnElapsed;
        }

        private void TimerOnElapsed(object sender, ElapsedEventArgs elapsedEventArgs)
        {
            if (!IODirectory.Exists(_options.DirectoryPath)) 
                return;

            var mkvFiles = IODirectory.GetFiles(
                _options.DirectoryPath, 
                "*.mkv", 
                SearchOption.AllDirectories);

            Parallel.ForEach(mkvFiles, file =>
            {
                var mkvConvertor = new MkvConvertor(file);
                var destinationPath = string.Format("{0}.MP4", file);

                DirectoryMonitorLog.LogToEventViewer(string.Format("Processing File: {0}", file));
                mkvConvertor.Convert(destinationPath);
                DirectoryMonitorLog.LogToEventViewer(string.Format("Processing File Completed: {0}", file));

                MoveFileToFinishedDirectory(file);
            });
        }

        private void MoveFileToFinishedDirectory(string file)
        {
            var finishedDirectory = string.Format("{0}Finished", IODirectory.GetCurrentDirectory());
            var finishedFile = string.Format("{0}{1}", finishedDirectory, Path.GetFileName(file));
            if (!IODirectory.Exists(finishedDirectory))
                IODirectory.CreateDirectory(finishedDirectory);
            IOFile.Move(file, finishedFile);
        }

        public void Dispose()
        {
            _timer.Stop();
            _timer.Enabled = false;
            _timer.Dispose();
        }
    }
}
