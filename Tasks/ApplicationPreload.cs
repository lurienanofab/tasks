using System;
using System.IO;
using System.Reflection;
using System.Web.Hosting;
using System.Configuration;


namespace Tasks
{
    public class ApplicationPreload : IProcessHostPreloadClient
    {
        public void Preload(string[] parameters)
        {
            try
            {
                HangfireBootstrapper.Instance.Start();
                WriteLog("BackgroundJobServer started successfully.");
            }
            catch (Exception ex)
            {
                WriteLog(string.Format("***** ERROR: BackgroundJobServer failed to start. *****{0}{1}", Environment.NewLine, ex.ToString()));
            }
        }

        private void WriteLog(string message)
        {
            try
            {
                string logsFolder = ConfigurationManager.AppSettings["LogsFolder"];
                string logFileName = Path.Combine(logsFolder, "HangfirePreload.log");

                if (!Directory.Exists(logsFolder))
                    Directory.CreateDirectory(logsFolder);

                File.AppendAllText(logFileName, string.Format("[{0:yyyy-MM-dd HH:mm:ss}][AppDomain.CurrentDomain.Id = {1}] {2}{3}", DateTime.Now, AppDomain.CurrentDomain.Id, message, Environment.NewLine));
            }
            catch
            {
                // I give up...
            }
        }
    }
}