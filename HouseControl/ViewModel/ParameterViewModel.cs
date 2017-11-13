using System;
using System.Collections.Generic;
using System.Linq;
using Facade;
using Model;
using ViewModelBase;

namespace ViewModel
{
    public class ParameterViewModel : EntytyObjectVM<Parameter>, IConditionSource, IPublicParam
    {
        public ParameterViewModel(IServiceContainer container, Models dataBase, Parameter model)
            : base(container, dataBase, model)
        {
            if (!IsFake && Model.ID == Parameter.CurrentTimeId)
                Use<ITimerSerivce>().Subsctibe(this, UpdateTime, 1000, true);
        }

        public override void AddedToPool()
        {
            base.AddedToPool();
            if (IsFake)
                return;
            Use<IPool>().GetViewModels<ParameterViewModel>().ForEach(a => a.OnParamsChanged());
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
                Model.Value = value;
                OnPropertyChanged();
            }
        }

        public IEnumerable<ParameterTypeViewModel> ParamTypes => Use<IPool>().GetViewModels<ParameterTypeViewModel>();
        public IEnumerable<ParameterCategoryVm> Categories => Use<IPool>().GetViewModels<ParameterCategoryVm>();
        public IEnumerable<IConditionSource> Sensors => Use<IPool>().GetViewModels<SensorViewModel>().Union(new[] { (IConditionSource)EmptyValue.Instance }) ;

        public ParameterTypeViewModel ParamType
        {
            get { return Use<IPool>().GetDBVM<ParameterTypeViewModel>(Model.ParameterType); }
            set
            {
                value.LinkParam(Model);
            }
        }
        public ParameterCategoryVm Category
        {
            get { return Use<IPool>().GetDBVM<ParameterCategoryVm>(Model.ParameterCategory); }
            set
            {
                value.LinkParam(Model);
                OnPropertyChanged();
            }
        }
        public IConditionSource Sensor
        {
            get { return Model.Sensor==null?EmptyValue.Instance: (IConditionSource)Use<IPool>().GetDBVM<SensorViewModel>(Model.Sensor); }
            set
            {
                if (value == EmptyValue.Instance)
                {
                    Model.Sensor = null;
                }
                else
                {
                    ((SensorViewModel)value).LinkParam(Model);
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
                    : Use<IPool>().GetDBVM<ParameterViewModel>(Model.NextParameter);
            }
            set
            {
                if (value == EmptyValue.Instance)
                    Model.NextParameter = null;
                else
                    Model.NextParameter = (value as ParameterViewModel).Model;
                OnPropertyChanged(()=>NextParam);
            }
        }

        public void OnParamsChanged()
        {
            OnPropertyChanged(()=>AllParams);
        }
        public IEnumerable<IConditionSource> AllParams
        {
            get
            {
                return
                    Use<IPool>()
                        .GetViewModels<ParameterViewModel>()
                        .Cast<IConditionSource>()
                        .Except(new[] {this})
                        .Union(new[] {EmptyValue.Instance});
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
            Use<IPool>().GetViewModels<ParameterViewModel>().ForEach(a => a.OnParamsChanged());
        }

        public ParameterProxy GetProxy()
        {
            var param = ParameterProxy.FromDBParameter(Model);
            param.ActualValue = Sensor==null?Value:Sensor.Value;
            return param;
        }
    }

    public interface IPublicParam
    {
        Type ValueType { get; }
        string Value { get; set; }
        IEnumerable<ParameterTypeViewModel> ParamTypes { get; }
        IEnumerable<ParameterCategoryVm> Categories { get; }
        IEnumerable<IConditionSource> Sensors { get; }
        ParameterTypeViewModel ParamType { get; set; }
        ParameterCategoryVm Category { get; set; }
        IConditionSource Sensor { get; set; }
        string Name { get; set; }
        string Description { get; set; }
        string ButtonDescription { get; set; }

        bool IsEditable { get; }
        bool IsPublic { get; set; }
        IConditionSource NextParam { get; set; }
        IEnumerable<IConditionSource> AllParams { get; }
        string Ident { get; }
    }
}