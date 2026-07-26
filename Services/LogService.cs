using System;
using System.IO;
namespace OrgCheck.Services
{
    public class LogService
    {
        public LogService()
        {
        }

        public void Log(Exception ex)
        {
            var logFile = Path.Combine(AppContext.BaseDirectory,"Log","Log_"+DateTime.Now.ToString("dd_MM_yyyy")+".txt");
            if (!File.Exists(logFile))
                File.Create(logFile);
            File.AppendAllText(logFile, "-----------------------------------------------" + Environment.NewLine);
            File.AppendAllText(logFile, ex.ToString() + Environment.NewLine);
        }
    }
}
