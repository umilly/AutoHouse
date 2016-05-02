using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Facade;

namespace Common
{
    public class EventLogger:ServiceBase,ILog
    {
        public void Log(LogCategory network, string message)
        {
            var show= $"[{network}] [{DateTime.Now.ToLongDateString()}]:'{message}'";
            Console.WriteLine(show);
            //Use<IViewService>().ShowMessage(show);
        }
    }
}
