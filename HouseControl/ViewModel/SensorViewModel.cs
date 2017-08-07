using System;
using System.Collections.Generic;
using System.Linq;
using Facade;
using Model;
using ViewModel;
using ViewModelBase;

namespace ViewModel
{
    public class SensorViewModel : LinkedObjectVM<Sensor>, IConditionSource
    {
        public SensorViewModel(IServiceContainer container, Models dataBase, Sensor model)
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
}