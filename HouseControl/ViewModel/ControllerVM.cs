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
    public class ControllerVM:LinkedObjectVM<Controller>
    {
        private readonly Dictionary<int, string> _values=new Dictionary<int, string>();
        private bool _isConnected;
        private static Dictionary<string, SensorType> _cahedTypes;
        public ControllerVM(IServiceContainer container,Models dataBase,Controller controller) : base(container,dataBase,controller)
        {
            fillSensorTypes();
            _contextMenu.Add(new CustomContextMenuItem("Добавить устройство",new CommandHandler(AddDevice)));
        }

        private void AddDevice(bool b)
        {
            var newDev=Use<IPool>().CreateDBObject<CustomDeviceViewModel>();
            newDev.LinkTo(Model);
            newDev.Name = "Устройство";
            OnPropertyChanged(()=>Children);
        }

        private void fillSensorTypes()
        {
            if (_cahedTypes != null || Context == null)
                return;
            _cahedTypes = new Dictionary<string, SensorType>();
            Context.SensorTypes.ForEach(a => _cahedTypes[a.Key] = a);
        }

        public override ITreeNode Parent => Use<IPool>().GetViewModels<DevicesViewModel>().FirstOrDefault();

        public override IEnumerable<ITreeNode> Children
        {
            get { return Use<IPool>().GetViewModels<SensorViewModel>()
                    .Cast<ITreeNode>()
                    .Union(Use<IPool>().GetViewModels<CustomDeviceViewModel>())
                    .Where(a => a.Parent == this); }
        }

        public Dictionary<int, string> Values => _values;

        public override string Name
        {
            get { return Model.Name; }
            set { Model.Name=value; }
        }

        public override string Value { get; set; }

        public override bool IsConnected
        {
            get { return _isConnected; }
            set
            {
                _isConnected = value; 
                OnPropertyChanged();
            }
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

        public override bool Validate()
        {
            return IP != null && Name != null;
        }

        public IEnumerable<Controller> GetControllers()
        {
            return Context.Devices.OfType<Controller>().ToList();
        }

        private string Url => $"http://{IP}:{Port}";

        public string MessageFind
        {
            get { return $"Найдено {Model.Sensors.Count} сенсоров"; }
        }

        public async void Update()
        {
            var task = Use<INetworkService>().AsyncRequest(Url);
            await task;
            ParseSensorsValues(task.Result);
        }

        private void ParseSensorsValues(string result)
        {
            if (string.IsNullOrEmpty(result))
            {
                IsConnected = false;
                return;
            }
            IsConnected = true;
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
                var value = line.Split('_').Last().Trim();
                _values[index] = value;
            }
        }

        public async Task FindSensors()
        {
//            var res = @"1_11_sens_temp_d_ 27.50 
//2_11_sens_hum_d_ 68.10 
//3_11_sens_temp_d_ 27.20 
//4_11_sens_hum_d_ 49.20 
//5_11_sens_pir_d_ 0 
//6_11_sens_pir_d_ 0 
//7_11_sens_temp_d_ 0.00 
//8_11_sens_hum_d_ 0.00 
//9_11_sens_mq2_a_ 232 
//10_12_sens_temp_d_ 0.00 
//11_12_sens_hum_d_ 0.00 
//12_12_sens_temp_d_ 0.00 
//13_12_sens_hum_d_ 0.00 
//14_12_sens_mq2_a_ 226 ";
            var task = Use<INetworkService>().AsyncRequest(Url);
            await task;
            ParseConrollerSensors(task.Result);
            ParseSensorsValues(task.Result);
            OnPropertyChanged(()=>Children);
        }

        private void ParseConrollerSensors(string result)
        {
            var lines=result.Split(new[] {"<br>", "\r","\n"},StringSplitOptions.RemoveEmptyEntries);
            int num = 0;
            foreach (var line in lines)
            {
                var key = _cahedTypes.Keys.FirstOrDefault(a => line.Contains(a));
                if (key != null)
                {
                    var sensorValues = line.Split('_');
                    var slotNum=  int.Parse(sensorValues[0]);
                    var zoneNum = int.Parse(sensorValues[1]);
                    var found = Use<IPool>().GetViewModels<SensorViewModel>().FirstOrDefault(a => a.Parent == this && a.Slot == slotNum);
                    if (found==null)
                    {
                        var newSensor = Use<IPool>().CreateDBObject<SensorViewModel>();
                        var zone = GetOrCreateZone(zoneNum);
                        newSensor.Init(_cahedTypes[key], slotNum, Model, "Датчик " + ++num);
                        newSensor.Zone = zone;
                    }
                }
            }
            Context.SaveChanges();
        }

        private ZoneViewModel GetOrCreateZone(int zoneNum)
        {
            var zone= Use<IPool>().GetViewModels<ZoneViewModel>().FirstOrDefault(a => a.Key == zoneNum.ToString());
            if (zone == null)
            {
                zone = Use<IPool>().CreateDBObject<ZoneViewModel>();
                zone.Key = zoneNum.ToString();
                zone.Name = "Зона " + zone.Key;
            }
            return zone;
        }
    }
}
