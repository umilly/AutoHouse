using Facade;
using Model;
using ViewModelBase;

namespace ViewModel
{
    public class ParameterTypeViewModel : EntytyObjectVM<ParameterType>
    {
        public ParameterTypeViewModel(IServiceContainer container, Models dataBase, ParameterType model) : base(container, dataBase, model)
        {
        }

        public string Name => Model.Name;

        public override bool Validate()
        {
            return true;
        }

        public void LinkParam(Parameter model)
        {
            model.ParameterType = Model;
        }

        public override string ToString()
        {
            return Name;
        }

        public void LinkDeviceParam(DeviceParameterTypeLink model)
        {
            model.ParameterType = Model;
        }
    }
}