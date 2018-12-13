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
        //private bool _isConnected;
        private static Dictionary<string, SensorType> _cahedTypes;
        public ControllerBase(IServiceContainer container,T controller) : base(container,controller)
        {
            fillSensorTypes();
            _contextMenu.Add(new CustomContextMenuItem("Добавить устройство",new CommandHandler(AddDevice)));
            _contextMenu.Add(new CustomContextMenuItem("Добавить сенсор", new CommandHandler(AddSensor)));
        }

        private void AddSensor(bool obj)
        {
            var newSensor = Use<IPool>().CreateDBObject<SecondTypeSensor>();
            var zone = Use<IPool>().GetViewModel<ZoneViewModel>(1);//GetOrCreateZone(zoneNum);
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
        
        private async Task fillSensorTypes()
        {
            if (_cahedTypes != null || Context == null)
                return;
            _cahedTypes = new Dictionary<string, SensorType>();
            var types=await Context.QueryModels<SensorType>(type => true);
            types.ForEach(a => _cahedTypes[a.Key] = a);
        }

        public override void LinklToParent(ITreeNode Parent)
        {
            throw new NotImplementedException();
        }

        public override ITreeNode Parent => Use<IPool>().GetViewModels<DevicesViewModel>().FirstOrDefault();

        public override IEnumerable<ITreeNode> Children
        {
            get { return Use<IPool>().GetViewModels<FirstTypeSensor>()
                .Cast<ITreeNode>()
                .Union(Use<IPool>().GetViewModels<SecondTypeSensor>())
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
            get { return VMState== VMState.Positive ? "+" : "-"; }
            set { }
        }

        private TimeSpan ConnectonTimeOut = new TimeSpan(0, 1, 0);

        public override VMState VMState
        {
            get { return DateTime.Now - _lastUpdate < ConnectonTimeOut ? VMState.Positive : VMState.Negative; }
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

        private string Url => $"http://{IP}:{Port}";

        public string MessageFind
        {
            get { return $"Всего в контроллер включено {Model.Sensors.Count} сенсоров"; }
        }

        public async void Update()
        {
            if(string.IsNullOrEmpty(IP)||Port<=0||Port>65535)
                return;
            using (var task = Use<INetworkService>().AsyncRequest(Url))
            {
                await task;
                ParseSensorsValues(task.Result);
            }
        }

        private  void ParseSensorsValues(string result)
        {
            if (string.IsNullOrEmpty(result))
            {
                //VMState = false;
                return;
            }
            UpdateStatus();
            //VMState = true;
            var lines = result.Split(new[] { "<",">"}, StringSplitOptions.RemoveEmptyEntries);
            HashSet<int> changed = new HashSet<int>();
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
                _values.TryGetValue(index, out var prevVal);
                if (prevVal != value)
                {
                    changed.Add(index);
                }
                _values[index] = value;
            }
            var sensors = Use<IPool>().GetViewModels<FirstTypeSensor>()
                .Where(a => a.Parent == this && changed.Contains(a.Slot));
            Use<IReactionService>().Check(sensors.Cast<IViewModel>().ToArray());
        }

        private Dictionary<string,SecondTypeSensor> _sensorByName=new Dictionary<string, SecondTypeSensor>();
        public void SetSensorsValues(Dictionary<string,string> sensorValues)
        {
            var updateAll = VMState != VMState.Positive;
            foreach (var sensorValue in sensorValues)
            {
                if (!_sensorByName.ContainsKey(sensorValue.Key))
                {
                    var sensor = Use<IPool>().GetViewModels<SecondTypeSensor>()
                        .FirstOrDefault(a => a.Parent == this && a.InternalName == sensorValue.Key);
                    if (sensor == null)
                    {
                        Use<ILog>().Log(LogCategory.Debug, $"для контроллера {IP} попытка устновить значение несуществующего датчика {sensorValue.Key}");
                        continue;
                    }
                    _sensorByName[sensorValue.Key] = sensor;
                }
                _sensorByName[sensorValue.Key].Value= sensorValue.Value;
            }
            UpdateStatus();
            var sensors = updateAll?
                 Use<IPool>().GetViewModels<SecondTypeSensor>()
                     .Where(a => a.Parent == this):
                sensorValues.Keys.Where(a=> _sensorByName.ContainsKey(a)).Select(a => _sensorByName[a]);
            Use<IReactionService>()
                .Check(sensors.ToArray());
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
                        var zone = Use<IPool>().GetViewModel<ZoneViewModel>(1);//GetOrCreateZone(zoneNum);
                        newSensor.Init(_cahedTypes[key], slotNum, Model, "Датчик " + ++num);
                        newSensor.Zone = zone;
                    }
                }
            }
            Context.SaveContextImmediate();
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