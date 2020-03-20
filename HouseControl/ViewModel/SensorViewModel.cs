using System;
using System.Collections.Generic;
using System.Linq;
using Facade;
using Model;
using ViewModel;
using ViewModelBase;

namespace ViewModel
{
    
    public abstract class SensorViewModel<T> : LinkedObjectVm<T>,ISensorVM where T: Sensor
    {
        public SensorViewModel(IServiceContainer container,  T model)
            : base(container, model)
        {
            Children = Enumerable.Empty<ITreeNode>();
        }

        IHaveID ISensorVM.Model { get=>Model; }

        public override bool Validate()
        {
            return Model.Controller != null
                   && Model.ContollerSlot > 0
                   && Model.SensorType != null
                   && Model.Name != null
                ;
        }

        public override void LinklToParent(ITreeNode Parent)
        {
            //throw new NotImplementedException();
        }
        public override Type ParentType { get { return typeof(ControllerVM); } }
        public override ITreeNode Parent => Use<IPool>().GetOrCreateDBVM<ControllerVM>(Model.Controller);


        public override IEnumerable<ITreeNode> Children { get; }

        public override string Name
        {
            get { return Model.Name; }
            set
            {
                Model.Name = value; 
                OnPropertyChanged();
            }
        }

        public Type ValueType
        {
            get { return typeof (float); }
        }



        public override VMState VMState
        {
            get
            {
                return Parent != null && (Parent as ControllerVM).Values.ContainsKey(Model.ContollerSlot)
                    ? VMState.Positive
                    : VMState.Negative;
            }

        }

        public void Init(SensorType st, int slotNum, Controller controller, string name)
        {
            Model.SensorType = st;
            Model.ContollerSlot = slotNum;
            Model.Controller = controller;
            Model.Name = name;
        }

        public void UpdateValue()
        {
            OnPropertyChanged(() => Value);
        }

        public void LinkCondition(Condition model)
        {
            model.Sensor = Model;
        }

        public string SourceName => $"Sens: {Name}";

        public int Slot => Model.ContollerSlot;

        public ZoneViewModel Zone
        {
            get { return Use<IPool>().GetOrCreateDBVM<ZoneViewModel>(Model.Zone); }
            set
            {
                value.LinkSensor(Model);
                OnPropertyChanged();
            }
        }

        public SensorTypeViewModel SensorType
        {
            get => Use<IPool>().GetOrCreateDBVM<SensorTypeViewModel>(Model.SensorType);
            set
            {
                value.LinkSensor(Model); 
                OnPropertyChanged();
                OnPropertyChanged(nameof(ValueRange));
            }
        }

        public IEnumerable<SensorTypeViewModel> AllSensorTypes => Use<IPool>().GetViewModels<SensorTypeViewModel>();
        public string ValueRange => $"от {Model.SensorType.MinValue} до {Model.SensorType.MaxValue}";

        public IEnumerable<ZoneViewModel> Zones => Use<IPool>().GetViewModels<ZoneViewModel>();

        public void LinkSetComand(ParametrSetCommand model)
        {
            model.Sensor = Model;
        }

        public void LinkParam(Parameter model)
        {
            model.Sensor = Model;
        }

    }

    public interface ISensorVM: IConditionSource, IEntityObjectVM
    {
        IHaveID Model { get; }
        bool Validate();
        void LinklToParent(ITreeNode Parent);
        Type ParentType { get; }
        ITreeNode Parent { get; }
        IEnumerable<ITreeNode> Children { get; }
        string Name { get; set; }
        Type ValueType { get; }
        string Value { get; set; }
        VMState VMState { get; }
        string SourceName { get; }
        int Slot { get; }
        ZoneViewModel Zone { get; set; }
        SensorTypeViewModel SensorType { get; }
        string ValueRange { get; }
        IEnumerable<ZoneViewModel> Zones { get; }
        void Init(SensorType st, int slotNum, Controller controller, string name);
        void UpdateValue();
        void LinkCondition(Condition model);
        void LinkSetComand(ParametrSetCommand model);
        void LinkParam(Parameter model);
    }

    public class SensorTypeViewModel : EntityObjectVm<SensorType>
    {
        public SensorTypeViewModel(IServiceContainer container, SensorType model) : base(container, model)
        {
        }
        public string Name
        {
            get => Model.Name;
        }

        public override bool Validate()
        {
            return true;
        }

        public void LinkSensor(Sensor model)
        {
            model.SensorType=Model;
        }
    }
}