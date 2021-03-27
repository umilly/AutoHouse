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
            

            Log(network, e.ToLog(Use<IGlobalParams>().LogLevel), showMesageBox);
        }
        
        public void LogNetException(Exception e, string prefix)
        {
            if (e is AggregateException && e.InnerException is WebException &&
                e.InnerException.InnerException != null && e.InnerException.InnerException is SocketException)
            {
                e = e.InnerException.InnerException;
            }
            Log(LogCategory.Network, prefix);
            if(Use<IGlobalParams>().LogLevel>1)
                Log(LogCategory.Network, e);
        }
    }
    public static class ExceptionExtension
    {
        public static string ToLog(this Exception ex, int logLevel)
        {
            var str =
                "\r\n__________________________________________________________________________________________________________________\r\n";
            while (ex != null)
            {
                str += $"{ex.Message}\r\n";
                if (logLevel< 1)
                    break;
                if (logLevel > 1)
                    str += $"{ex.StackTrace}";
                ex = ex.InnerException;
                if (ex != null)
                    str += "<<<<<<<<<<<<<<<<<\r\n ";
            }
            str +=
                "__________________________________________________________________________________________________________________\r\n";
            return str;
        }
    }
}
