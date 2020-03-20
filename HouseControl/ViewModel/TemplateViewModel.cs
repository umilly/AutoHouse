using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Facade;
using Model;
using ViewModelBase;

namespace ViewModel
{
    public class TemplateViewModel : LinkedObjectVm<Template>, IConditionParent
    {
        public TemplateViewModel(IServiceContainer container, Template model)
            : base(container, model)
        {

        }

        public override Type ParentType
        {
            get { return typeof(ScenarioViewModel); }
        }


        public ScenarioViewModel Scenario
        {
            get { return Use<IPool>().GetOrCreateDBVM<ScenarioViewModel>(Model.Scenario); }
        }

        public override bool Validate()
        {
            return !string.IsNullOrEmpty(Model.Name);
        }
        public int TemplateMode { get=>Model.TemplateMode;
            set
            {
                Model.TemplateMode = value;
                OnPropertyChanged();
            }
        }
        public List<SensorWrapper> AllSensors
        {
            get => Use<IPool>().GetViewModels<ISensorVM>().Select(a=>new SensorWrapper((Sensor)a.Model,Model,Use<IContext>())).ToList();
        }

      
        public override string Name
        {
            get { return Model.Name; }
            set
            {
                Model.Name = value;
                OnPropertyChanged();
            }
        }

        public override void LinklToParent(ITreeNode newParent)
        {
            
            if (!(newParent is ScenarioViewModel))
                throw new InvalidEnumArgumentException("reaction's parent must be scenario");
            (newParent as ScenarioViewModel).LinkTemplate(Model);
        }

        public override ITreeNode Parent => Scenario;

        public override IEnumerable<ITreeNode> Children => Enumerable.Empty<ITreeNode>();

        public override string Value
        {
            get { return IsActive ? string.Empty : "Disabled"; }
            set { }
        }

        public override VMState VMState
        {
            get => VMState.Default;
        }

        public bool IsActive
        {
            get { return Model.IsActive; }
            set
            {
                Model.IsActive = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(LastUpdateMs));
                OnPropertyChanged(nameof(Value));
                Use<IReactionService>().Check(this);
            }
        }

        public void Link(Scenario model)
        {
            Model.Scenario = model;
        }

        public override int LastUpdateMs
        {
            get { return IsActive ? base.LastUpdateMs : -1; }
        }

        public string Description
        {
            get { return "Шаблон регулирования климата"; }
        }

        public void Check()
        {
            //var conditions = Use<IPool>()
            //    .GetViewModels<ConditionViewModel>()
            //    .Where(a => a.Parent == this);

            //if (conditions.All(a => a.CheckComplete()))
            //{
            //    SendCommands();
            //}
        }

        private void SendCommands()
        {
            //Use<IPool>()
            //    .GetViewModels<CommandViewModel>()
            //    .Where(a => a.Parent == this)
            //    .ForEach(a => a.Execute());
            //Use<IPool>()
            //    .GetViewModels<ParameterSetCommandVm>()
            //    .Where(a => a.Parent == this)
            //    .ForEach(a => a.Execute());

        }
        public List<ParameterWrapper> SelectedParams
        {
            get => Model.TemplateParameters.Select(a=>new ParameterWrapper(Use<IPool>(), a)).ToList();
        }
        public List<DevicePairWrapper> SelectedDevicePairs
        {
            get => Model.TemplatedDevicePairs.Select(a => new DevicePairWrapper(Use<IPool>(), a)).ToList();
        }

        public List<ModeWrapper> AllModes
        {
            get { return ModeWrapper.ClimatModes; }
        }
        public ModeWrapper SelectedMode
        {
            get { return AllModes.First(a => a.Value == Model.TemplateMode); }
            set
            {
                Model.TemplateMode = value.Value;
                OnPropertyChanged();
            }
        }

        public void AddParam()
        {
            var link = Use<IContext>().CreateModel<TemplateParameter>();
            link.Parameter = null;
            link.Template = Model;
            Model.TemplateParameters.Add(link);
            link.Order = Model.TemplateParameters.Count;
            OnPropertyChanged(() => SelectedParams);
        }

        public void DeleteParams()
        {
            var toDelete = (Model?.TemplateParameters ?? Enumerable.Empty<TemplateParameter>()).ToList();
            foreach (var modelTemplateParameter in toDelete)
            {
                Use<IContext>().Delete(modelTemplateParameter);
            }
            Model.TemplateParameters.Clear();
            OnPropertyChanged(() => SelectedParams);
        }
        public void AddDevicePairs()
        {
            var link = Use<IContext>().CreateModel<TemplatedDevicePair>();
            link.Template = Model;
            Model.TemplatedDevicePairs.Add(link);
            link.Order = Model.TemplateParameters.Count;
            OnPropertyChanged(() => SelectedDevicePairs);
        }

        public void DeleteDevicePairs()
        {
            var toDelete = (Model?.TemplatedDevicePairs ?? Enumerable.Empty<TemplatedDevicePair>()).ToList();
            foreach (var modelTemplateParameter in toDelete)
            {
                Use<IContext>().Delete(modelTemplateParameter);
            }
            Model.TemplatedDevicePairs.Clear();
            OnPropertyChanged(() => SelectedDevicePairs);
        }
    }

    public class ModeWrapper
    {
        public string Name { get; set; }
        public int Value { get; set; }

        private static List<ModeWrapper> _climatModes = new List<ModeWrapper>(new[]
        {
            new ModeWrapper() {Name = "Нагрев", Value = 0}, 
            new ModeWrapper {Name = "Сбалансированный", Value = 1},
            new ModeWrapper() {Name = "Охлаждение", Value = 2},
        });
        public static List<ModeWrapper> ClimatModes
        {
            get
            {
                return _climatModes;
            }
        }
    }


    public class SensorWrapper
    {
        private Sensor _sensor;
        private Template _template;
        private readonly IContext _db;

        public SensorWrapper(Sensor sensor, Template template, IContext db )
        {
            _sensor = sensor;
            _template = template;
            _db = db;
        }
        public string Name
        {
            get => _sensor.Name;
        }
        public bool IsLinked
        {
            get { return _template.TemplateSensors.Any(a => a.Sensor == _sensor);}
            set
            {
                if (value)
                {
                    var ts=_db.CreateModel<TemplateSensor>();
                    _template.TemplateSensors.Add(ts);
                    ts.Sensor = _sensor;
                    ts.Template = _template;
                    ts.Order = _template.TemplateSensors.Max(a => a.Order) + 1;
                }
                else
                {
                    var toDel=_template.TemplateSensors.FirstOrDefault(a => a.Template == _template && a.Sensor == _sensor);
                    if (toDel != null)
                    {
                        _db.Delete(toDel);
                        _template.TemplateSensors.Remove(toDel);
                    }
                }
            }
        }

    }

    public class ParameterWrapper
    {
        private readonly IPool _pool;
        private readonly TemplateParameter _parameter;
     
        public ParameterWrapper(IPool pool, TemplateParameter parameter)
        {
            _pool = pool;
            _parameter = parameter;
        }

        public List<ParameterViewModel> AllParameters
        {
            get {return  _pool.GetViewModels<ParameterViewModel>().ToList(); }
        }

        public ParameterViewModel SelectedParameter
        {
            get { return _pool.GetOrCreateDBVM<ParameterViewModel>(_parameter.Parameter); }
            set
            {
                _parameter.Parameter = value.Model; 
                
            }
        }
    }
    public class DevicePairWrapper
    {
        private readonly IPool _pool;
        private readonly TemplatedDevicePair _parameter;


        public DevicePairWrapper(IPool pool, TemplatedDevicePair parameter)
        {
            _pool = pool;
            _parameter = parameter;
        }

        public List<ISensorVM> AllSensors
        {
            get { return _pool.GetViewModels<ISensorVM>().Where(a=> _parameter.Template.Scenario.Zones.Contains(a.Zone.Model)).ToList(); }
        }
        public List<CustomDeviceViewModel> AllDevices
        {
            get { return _pool.GetViewModels<CustomDeviceViewModel>().ToList(); }
        }
        public ISensorVM SelectedSensor
        {
            get { return _pool.GetOrCreateDBVM<ISensorVM>(_parameter.Sensor); }
            set
            {
                _parameter.Sensor = (Sensor)value.Model;

            }
        }
        public CustomDeviceViewModel SelectedDevice
        {
            get { return _pool.GetOrCreateDBVM<CustomDeviceViewModel>((CustomDevice)_parameter.Device); }
            set
            {
                _parameter.Device = value.Model;

            }
        }
    }
}