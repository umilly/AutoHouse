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
    public class ControllerVM:EntytyObjectVM<Controller>
    {
        public ControllerVM(IServiceContainer container,Models dataBase,Controller controller) : base(container,dataBase,controller)
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
            var lines=result.Split(new[] {"<br>", "\r","\n"},StringSplitOptions.RemoveEmptyEntries);
            int num = 0;
            foreach (var line in lines)
            {
                var type= Context.SensorTypes.FirstOrDefault(a => line.Contains(a.Key));
                if (type != null)
                {
                    var sensor = Context.Sensors.Create();
                    sensor.SensorType = type;
                    sensor.ContollerSlot =int.Parse(line.Split('_').First());
                    sensor.Controller = Model;
                    sensor.Name = "Датчик " + ++num;
                    Context.Sensors.Add(sensor);
                }
            }
            Context.SaveChanges();

        }
        
    }
    
}

public class SensorType
{
    public string Key { get; set; }
}
