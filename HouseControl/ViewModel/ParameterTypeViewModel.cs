using System;
using Facade;
using Model;
using ViewModelBase;

namespace ViewModel
{
    public class ParameterTypeViewModel : EntityObjectVm<ParameterType>
    {
        public ParameterTypeViewModel(IServiceContainer container,  ParameterType model) : base(container,  model)
        {
            if(IsFake)
                return;
            Type = TypeAssociationAttribute.GetType((ParameterTypeValue) model.ID);
            ParameterTypeValue = (ParameterTypeValue) model.ID;
        }
        public ParameterTypeValue ParameterTypeValue { get; set; }
        public string Name => Model.Name;

        public Type Type { get;private set; }

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