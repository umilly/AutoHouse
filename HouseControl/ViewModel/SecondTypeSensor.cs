using Facade;
using Model;

namespace ViewModel
{
    public class SecondTypeSensor : SensorViewModel<CustomSensor>
    {
        public SecondTypeSensor(IServiceContainer container, CustomSensor model)
            : base(container,model) { }

    }
}