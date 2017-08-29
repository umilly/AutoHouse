using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
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

        public void Log(LogCategory network, Exception e, bool showMesageBox = false)
        {
            var excep = e;
            var str = "__________________________________________________________________________________________________________________\r\n";
            while (e!=null)
            {
                str += $"{e.Message}\r\n {e.StackTrace}\r\n_________________________________\r\n";
                e = e.InnerException;
            }
            str += "__________________________________________________________________________________________________________________\r\n";

            Log(network, str, showMesageBox);
        }

        public void LogNetException(Exception e, string prefix)
        {
            if (e is AggregateException && e.InnerException is WebException &&
                e.InnerException.InnerException != null && e.InnerException.InnerException is SocketException)
            {
                Log(LogCategory.Network, prefix + e.InnerException.InnerException.Message);
            }
            else
            {
                Log(LogCategory.Network, prefix);
                Log(LogCategory.Network, e);
            }
        }

    }
}
