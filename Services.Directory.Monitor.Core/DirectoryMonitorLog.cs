using System;
using System.Diagnostics;
using System.Threading.Tasks;

namespace Services.Directory.Monitor.Core
{
    public class DirectoryMonitorLog
    {
        public static void LogToEventViewer(string message)
        {
            var logMessage = string.Format("{0}: {1}", DateTime.Now, message);

            Task.Factory.StartNew(() => EventLog.WriteEntry("DirectoryMonitorService", logMessage));
        }
    }
}