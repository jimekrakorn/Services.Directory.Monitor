using System.ServiceProcess;
using Services.Directory.Monitor.Core;

namespace Services.Directory.Monitor
{
    public partial class DirectoryMonitorService : ServiceBase
    {
        private DirectoryMonitor _directoryMonitor;

        public DirectoryMonitorService()
        {
            InitializeComponent();

        }

        protected override void OnStart(string[] args)
        {
            const int monitorInterval = 1000;
            const string directoryPath = @"C:\Users\Chad\SkyDrive\Downloads";

            var options = new DirectoryMonitorOptions
            {
                DirectoryPath = directoryPath,
                DirectoryMonitorInterval = monitorInterval
            };
            _directoryMonitor = new DirectoryMonitor(options);
        }

        protected override void OnStop()
        {
            _directoryMonitor.Dispose();
        }
    }
}
