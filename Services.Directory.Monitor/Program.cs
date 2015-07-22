using System.ServiceProcess;

namespace Services.Directory.Monitor
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main()
        {
            var servicesToRun = new ServiceBase[] 
            { 
                new DirectoryMonitorService() 
            };
            ServiceBase.Run(servicesToRun);
        }
    }
}
