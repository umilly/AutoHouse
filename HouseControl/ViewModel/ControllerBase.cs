using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Facade;
using Model;
using ViewModelBase;

namespace ViewModel
{
    public abstract class ControllerBase<T> : LinkedObjectVm<T> where T:Controller
    {
        private readonly Dictionary<int, string> _values=new Dictionary<int, string>();
        private bool _isConnected;
        private static Dictionary<string, SensorType> _cahedTypes;
        public ControllerBase(IServiceContainer container,Models dataBase,T controller) : base(container,dataBase,controller)
        {
            fillSensorTypes();
            _contextMenu.Add(new CustomContextMenuItem("Добавить устройство",new CommandHandler(AddDevice)));
            _contextMenu.Add(new CustomContextMenuItem("Добавить сенсор", new CommandHandler(AddSensor)));
        }

        private void AddSensor(bool obj)
        {
            var newSensor = Use<IPool>().CreateDBObject<FirstTypeSensor>();
            var zone = Use<IPool>().GetDBVM<ZoneViewModel>(1);//GetOrCreateZone(zoneNum);
            var sensors = Use<IPool>().GetViewModels<ISensorVM>().Where(a => a.Parent == this).ToList();
            int slot = sensors.Any() ? sensors.Max(a => a.Slot) : 0;
            newSensor.Init(_cahedTypes.First().Value, slot+1, Model, "Датчик");
            newSensor.Zone = zone;
            OnPropertyChanged(()=>Children);
        }

        private void AddDevice(bool b)
        {
            var newDev=CreateChildDev();
            newDev.LinkTo(Model);
            newDev.Name = "Устройство";
            OnPropertyChanged(()=>Children);
        }

        protected abstract ICustomDevice CreateChildDev();
        
        private void fillSensorTypes()
        {
            if (_cahedTypes != null || Context == null)
                return;
            _cahedTypes = new Dictionary<string, SensorType>();
            Context.SensorTypes.ForEach(a => _cahedTypes[a.Key] = a);
        }

        public override void LinklToParent(ITreeNode Parent)
        {
            throw new NotImplementedException();
        }

        public override ITreeNode Parent => Use<IPool>().GetViewModels<DevicesViewModel>().FirstOrDefault();

        public override IEnumerable<ITreeNode> Children
        {
            get { return Use<IPool>().GetViewModels<ISensorVM>()
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

        public override string Value
        {
            get { return IsConnected.Value ? "+" : "-"; }
            set { }
        }

        public override bool? IsConnected
        {
            get { return _isConnected; }
            set
            {
                _isConnected = value.Value;
                if (_isConnected)
                {
                    Update();
                    UpdateStatus();
                }
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
            get { return $"Всего в контроллер включено {Model.Sensors.Count} сенсоров"; }
        }

        public static int i = 0;
        public async void Update()
        {
            if(string.IsNullOrEmpty(IP)||Port<=0||Port>65535)
                return;
            i++;
            using (var task = Use<INetworkService>().AsyncRequest(Url))
            {
                await task;
                ParseSensorsValues(task.Result);
            }
            i--;
        }

        private void ParseSensorsValues(string result)
        {
            if (string.IsNullOrEmpty(result))
            {
                IsConnected = false;
                return;
            }
            IsConnected = true;
            var lines = result.Split(new[] { "<",">"}, StringSplitOptions.RemoveEmptyEntries);
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

        public  async Task FindSensors()
        {
            var task = Use<INetworkService>().AsyncRequest(Url);
            await task;
            ParseConrollerSensors(task.Result);
            ParseSensorsValues(task.Result);
            OnPropertyChanged(() => Children);
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
                    //var zoneNum = int.Parse(sensorValues[1]);
                    var found = Use<IPool>().GetViewModels<FirstTypeSensor>().FirstOrDefault(a => a.Parent == this && a.Slot == slotNum);
                    if (found==null)
                    {
                        var newSensor = Use<IPool>().CreateDBObject<FirstTypeSensor>();
                        var zone = Use<IPool>().GetDBVM<ZoneViewModel>(1);//GetOrCreateZone(zoneNum);
                        newSensor.Init(_cahedTypes[key], slotNum, Model, "Датчик " + ++num);
                        newSensor.Zone = zone;
                    }
                }
            }
            Context.SaveChanges();
        }

        //private ZoneViewModel GetOrCreateZone(int zoneNum)
        //{
        //    var zone= Use<IPool>().GetViewModels<ZoneViewModel>().FirstOrDefault(a => a.Key == zoneNum.ToString());
        //    if (zone == null)
        //    {
        //        zone = Use<IPool>().CreateDBObject<ZoneViewModel>();
        //        zone.Key = zoneNum.ToString();
        //        zone.Name = "Зона " + zone.Key;
        //    }
        //    return zone;
        //}
    }
}