using System;
using System.Threading.Tasks;
using Facade;
using Model;

namespace ViewModel
{
    public class SecondTypeSensor : SensorViewModel<CustomSensor>
    {
        public SecondTypeSensor(IServiceContainer container, CustomSensor model)
            : base(container,model) { }

        protected override Task OnCreate()
        {
            if (Model.ValueChangeDate == new DateTime())
            {
                Model.ValueChangeDate =DateTime.Now;
            }
            return base.OnCreate();
        }

        public override string Value
        {
            get => Model.LastValue??"-";
            set
            {
                Model.LastValue = value;
                UpdateStatus();
            }
        }

        public override VMState VMState { get=> Model.LastValue==null?VMState.Negative : VMState.Positive; }

        public string InternalName
        {
            get => Model.InnerName;
            set
            {
                Model.InnerName = value; 
                OnPropertyChanged();
            }
        }
    }
}