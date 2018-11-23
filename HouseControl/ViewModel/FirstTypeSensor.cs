using Facade;
using Model;

namespace ViewModel
{
    public class FirstTypeSensor : SensorViewModel<Sensor>
    {
        public FirstTypeSensor(IServiceContainer container,  Sensor model)
            : base(container, model) { }
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

    }
}