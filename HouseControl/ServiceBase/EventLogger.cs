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
        public void Log(LogCategory network, string message,bool showMessageBox=false)
        {
            var show= $"[{network}] [{DateTime.Now.ToLongDateString()}]:'{message}'";
            Console.WriteLine(show);
            if(showMessageBox)
                Use<IViewService>().ShowMessage(show);
        }
    }
}
