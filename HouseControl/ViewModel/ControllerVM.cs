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
    public class ControllerVM:EntytyObjectVM<Controller>,ITreeNode
    {
        private readonly Dictionary<int, string> _values=new Dictionary<int, string>();
        private static Dictionary<string, SensorType> _cahedTypes;
        public ControllerVM(IServiceContainer container,Models dataBase,Controller controller) : base(container,dataBase,controller)
        {
           
        }

        private void fillSensorTypes()
        {
            if (_cahedTypes == null && Context != null)
            {
                _cahedTypes = new Dictionary<string, SensorType>();
                Context.SensorTypes.ForEach(a => _cahedTypes[a.Key] = a);
            }
        }

        public ITreeNode Parent { get; }

        public IEnumerable<ITreeNode> Children
        {
            get { return Use<IPool>().GetViewModels<SensorViewModel>().Where(a => a.Parent == this); }
        }

        public Dictionary<int, string> Values => _values;

        public string Name
        {
            get { return Model.Name; }
            set { Model.Name=value; }
        }

        public string Value { get; }

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

        public override bool Validate()
        {
            return IP != null && Name != null;
        }

        protected override void OnDelete()
        {
            Children.Cast<IEntytyObjectVM>().ToList().ForEach(a=>a.Delete());
            base.OnDelete();
        }

        public IEnumerable<Controller> GetControllers()
        {
            return Context.Controllers.ToList();
        }

        private string Url => $"http://{IP}:{Port}";

        public async void Update()
        {
            var task = Use<INetworkService>().AsyncRequest(Url);
            await task;
            ParseSensorsValues(task.Result);
        }

        private void ParseSensorsValues(string result)
        {
            fillSensorTypes();
            var lines = result.Split(new[] { "<br>", "\r", "\n" }, StringSplitOptions.RemoveEmptyEntries);
            for (int i = 0; i < lines.Count(); i++)
            {
                var line = lines[i];
                var key = _cahedTypes.Keys.FirstOrDefault(a => line.Contains(a));
                if (key==null)
                {
                    continue;
                }
                var index = int.Parse(line.Split('_').First());
                line = lines[++i];
                var value = line.Split(' ').Last();
                _values[index] = value;
            }
        }

        public async void FindSensors()
        {
            var task = Use<INetworkService>().AsyncRequest(Url);
            await task;
            ParseConrollerSensors(task.Result);
            ParseSensorsValues(task.Result);
        }

        private void ParseConrollerSensors(string result)
        {
            fillSensorTypes();
            var lines=result.Split(new[] {"<br>", "\r","\n"},StringSplitOptions.RemoveEmptyEntries);
            int num = 0;
            foreach (var line in lines)
            {
                var key = _cahedTypes.Keys.FirstOrDefault(a => line.Contains(a));
                if (key != null)
                {
                    var newSensor = Use<IPool>().CreateDBObject<SensorViewModel>();
                    newSensor.Init(_cahedTypes[key], int.Parse(line.Split('_').First()), Model, "Датчик " + ++num);
                }
            }
            Context.SaveChanges();
        }
    }
}
