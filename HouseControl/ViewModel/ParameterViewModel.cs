using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Facade;
using Model;
using ViewModelBase;

namespace ViewModel
{
    public class ParameterViewModel : EntityObjectVm<Parameter>, IConditionSource, IPublicParam
    {
        public ParameterViewModel(IServiceContainer container,  Parameter model)
            : base(container, model)
        {
            if (!IsFake && Model.ID == Parameter.CurrentTimeId)
                Use<ITimerSerivce>().Subscribe(this, UpdateTime, 1000, true);
            else
            {
                TimerLogic();
            }
            
        }

        public void TimerLogic()
        {
            Use<ITimerSerivce>().UnSubsctibe(this);
            if (ParamType.ParameterTypeValue == ParameterTypeValue.Time)
            {
                DateTime.TryParse(Value, CultureInfo.InvariantCulture.NumberFormat,DateTimeStyles.AllowInnerWhite,out var time);
                var ts = (time.TimeOfDay-DateTime.Now.TimeOfDay);
                if (ts.TotalMilliseconds < 0)
                {
                    ts = (time.TimeOfDay.Add(new TimeSpan(1,0,0,0)) - DateTime.Now.TimeOfDay);
                }
                Use<ITimerSerivce>().Subscribe(this,Check,(int)ts.TotalMilliseconds+1001);
            }
        }

        private void Check()
        {
            Use<IReactionService>().Check(this);
            TimerLogic();
        }

        public override void AddedToPool()
        {
            base.AddedToPool();
            if (IsFake)
                return;
            Use<IPool>().GetViewModels<ParametersListViewModel>().ForEach(a => a.OnParamsChanged());
        }

        public Type ValueType => ParamType.Type;

        private void UpdateTime()
        {
            OnPropertyChanged(() => Value);
        }

        public override bool Validate()
        {
            return !string.IsNullOrEmpty(Model.Name) && Model.ParameterType != null;
        }

        public string Value
        {
            get { return Model.ID == Parameter.CurrentTimeId ? DateTime.Now.ToLongTimeString() : Model.Value; }
            set
            {
                if(Model.Value==value)
                    return;
                Model.Value = value;
                OnPropertyChanged();
                TimerLogic();
                Use<IReactionService>().Check(this);
            }
        }
        public ParameterTypeViewModel ParamType
        {
            get { return Use<IPool>().GetOrCreateDBVM<ParameterTypeViewModel>(Model.ParameterType); }
            set
            {
                value.LinkParam(Model);
                TimerLogic();
            }
        }
        public ParameterCategoryVm Category
        {
            get { return Use<IPool>().GetOrCreateDBVM<ParameterCategoryVm>(Model.ParameterCategory); }
            set
            {
                value.LinkParam(Model);
                OnPropertyChanged();
            }
        }
        public IConditionSource Sensor
        {
            get { return Model.Sensor==null?EmptyValue.Instance: (IConditionSource)Use<IPool>().GetOrCreateDBVM<ISensorVM>(Model.Sensor); }
            set
            {
                if (value == EmptyValue.Instance)
                {
                    Model.Sensor = null;
                }
                else
                {
                    ((ISensorVM)value).LinkParam(Model);
                }
                OnPropertyChanged();
            }
        }
        public string Name
        {
            get { return Model.Name; }
            set
            {
                Model.Name = value;
                OnPropertyChanged();
            }
        }
        public string Description
        {
            get { return Model.Description; }
            set
            {
                Model.Description = value;
                OnPropertyChanged();
            }
        }

        public bool IsEditable => Model.ID != Parameter.CurrentTimeId;

        public void LinkCondition(Condition model, bool firstParam)
        {
            if (firstParam)
            {
                model.Parameter1 = Model;
            }
            else
            {
                model.Parameter2 = Model;
            }
        }

        public string SourceName => $"Param: {Name}";

        public bool IsPublic
        {
            get { return Model.IsPublic; }
            set
            {
                Model.IsPublic = value;
                OnPropertyChanged();
            }
        }

        public IConditionSource NextParam
        {
            get
            {
                return Model.NextParameter == null
                    ? (IConditionSource) EmptyValue.Instance
                    : Use<IPool>().GetOrCreateDBVM<ParameterViewModel>(Model.NextParameter);
            }
            set
            {
                if (value == EmptyValue.Instance||value==this)
                    Model.NextParameter = null;
                else
                    Model.NextParameter = (value as ParameterViewModel).Model;
                OnPropertyChanged(()=>NextParam);
            }
        }
        

        public string Ident => ID.ToString();

        public string ButtonDescription
        {
            get => Model.ButtonDescription; set
            {
                Model.ButtonDescription = value;
                OnPropertyChanged();
            }
        }

        public void LinkDeviceParam(ComandParameterLink model)
        {
            model.Parameter = Model;
        }

        public void LinkDestSetParam(ParametrSetCommand model)
        {
            model.DestParameter = Model;
        }

        public void LinkSrcSetParam1(ParametrSetCommand model)
        {
            model.SrcParameter1 = Model;
        }
        public void LinkSrcSetParam2(ParametrSetCommand model)
        {
            model.SrcParameter2 = Model;
        }

        public override void Delete()
        {
            base.Delete();
            Use<IPool>().GetViewModels<ParametersListViewModel>().ForEach(a => a.OnParamsChanged());
        }

        public ParameterProxy GetProxy()
        {
            var param = ParameterProxy.FromDBParameter(Model);
            param.ActualValue = Sensor==null||Sensor is EmptyValue?Value:Sensor.Value;
            return param;
        }
    }

    public interface IPublicParam
    {
        Type ValueType { get; }
        string Value { get; set; }
        ParameterTypeViewModel ParamType { get; set; }
        ParameterCategoryVm Category { get; set; }
        IConditionSource Sensor { get; set; }
        string Name { get; set; }
        string Description { get; set; }
        string ButtonDescription { get; set; }

        bool IsEditable { get; }
        bool IsPublic { get; set; }
        IConditionSource NextParam { get; set; }
        string Ident { get; }
    }
}