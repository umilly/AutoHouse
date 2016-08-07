using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Facade;

namespace Common
{
    public class EventLogger:ServiceBase,ILog
    {
        public const string logFile = "log.txt";
        public const long maxSize = 100*1024*1024;
        public void Log(LogCategory network, string message,bool showMessageBox=false)
        {
            var show= $"[{network}] [{DateTime.Now.ToLongTimeString()}]:'{message}'";
            Console.WriteLine(show);
            try
            {
                var f=new FileInfo(logFile);
                if (f.Exists&& f.Length > maxSize)
                {
                    File.Delete(logFile);
                }
                File.AppendAllLines(logFile, new[] {show});
            }
            catch (Exception)
            {
                
            }
            if(showMessageBox)
                Use<IViewService>().ShowMessage(show);
        }
    }
}
