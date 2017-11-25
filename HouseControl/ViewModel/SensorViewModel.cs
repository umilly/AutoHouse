using System;
using System.Collections.Generic;
using System.Linq;
using Facade;
using Model;
using ViewModel;
using ViewModelBase;

namespace ViewModel
{
    
    public abstract class SensorViewModel<T> : LinkedObjectVM<T>,ISensorVM where T: Sensor
    {
        public SensorViewModel(IServiceContainer container, Models dataBase, T model)
            : base(container, dataBase, model)
        {
            Children = Enumerable.Empty<ITreeNode>();
        }

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
            throw new NotImplementedException();
        }
        public override Type ParentType { get { return typeof(ControllerVM); } }
        public override ITreeNode Parent => Use<IPool>().GetDBVM<ControllerVM>(Model.Controller);


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

        public override string Value
        {
            get
            {
                var p = Parent as ControllerVM;
                return (p != null && p.Values.ContainsKey(Model.ContollerSlot))
                    ? p.Values[Model.ContollerSlot]
                    : "-";
            }
            set { }
        }

        public override bool? IsConnected
        {
            get { return Parent != null && (Parent as ControllerVM).Values.ContainsKey(Model.ContollerSlot); }
            set { }
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
            get { return Use<IPool>().GetDBVM<ZoneViewModel>(Model.Zone); }
            set
            {
                value.LinkSensor(Model);
                OnPropertyChanged();
            }
        }

        public string SensorType => Model.SensorType.Name;

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

    public interface ISensorVM: IConditionSource, IEntytyObjectVM
    {
        bool Validate();
        void LinklToParent(ITreeNode Parent);
        Type ParentType { get; }
        ITreeNode Parent { get; }
        IEnumerable<ITreeNode> Children { get; }
        string Name { get; set; }
        Type ValueType { get; }
        string Value { get; set; }
        bool? IsConnected { get; set; }
        string SourceName { get; }
        int Slot { get; }
        ZoneViewModel Zone { get; set; }
        string SensorType { get; }
        string ValueRange { get; }
        IEnumerable<ZoneViewModel> Zones { get; }
        void Init(SensorType st, int slotNum, Controller controller, string name);
        void UpdateValue();
        void LinkCondition(Condition model);
        void LinkSetComand(ParametrSetCommand model);
        void LinkParam(Parameter model);
    }

    public class FirstTypeSensor : SensorViewModel<Sensor>
    {
        public FirstTypeSensor(IServiceContainer container, Models dataBase, Sensor model)
            : base(container, dataBase, model) { }

    }
    public class SecondTypeSensor : SensorViewModel<CustomSensor>
    {
        public SecondTypeSensor(IServiceContainer container, Models dataBase, CustomSensor model)
            : base(container, dataBase, model) { }

    }
}