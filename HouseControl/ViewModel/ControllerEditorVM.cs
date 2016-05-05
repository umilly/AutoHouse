using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Facade;
using Model;
using ViewModelBase;

namespace ViewModel
{
    public class ControllerEditorVM:ViewModelBase.EntytyObjectVM<Controller>
    {
        public ControllerEditorVM(IServiceContainer container) : base(container)
        {
        }

        public override int ID
        {
            get { return Model.Id; }
            set
            {
                if(Model!=null&&Model.Id==value)
                    return;
                OnCreate(value);
                OnPropertyChanged(()=>Name);
                OnPropertyChanged(() => IP);
                OnPropertyChanged(() => Port);
                OnPropertyChanged(() => ID);
            }
        }

        public string Name
        {
            get { return Model.Name; }
            set { Model.Name=value; }
        }

        public string IP
        {
            get { return Model.IP; }
            set { Model.IP=value; }
        }
        public int Port
        {
            get { return Model.Port; }
            set { Model.Port = value; }
        }

        protected override bool Validate()
        {
            return IP != null && Name != null;
        }

        public IEnumerable<Controller> GetControllers()
        {
            return Context.Controllers.ToList();
        }

        public async void Find()
        {
            var task = Use<INetworkService>().AsyncRequest(string.Format("http://{0}:{1}", IP, Port));
            await task;
            ParseConrollerResult(task.Result);
        }

        private void ParseConrollerResult(string result)
        {
            Regex ex=new Regex("");
            var lines=result.Split(new[] { '\r','\n'},StringSplitOptions.RemoveEmptyEntries);
            foreach (var line in lines)
            {
                var type = sensorTypes.FirstOrDefault(a => line.StartsWith(a.Key));
            }
            
        }
        private static List<SensorType>  sensorTypes=new List<SensorType>();

        private void FillSensors()
        {
            sensorTypes.Add(new SensorType());
        }
    }
    
}

public class SensorType
{
    public string Key { get; set; }
}
