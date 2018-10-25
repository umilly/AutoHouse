using System.Collections.Generic;
using System.Linq;
using Facade;
using Model;
using ViewModelBase;

namespace ViewModel
{
    public class DeviceParamLinkVM : EntityObjectVm<DeviceParameterTypeLink>
    {
        public DeviceParamLinkVM(IServiceContainer container,DeviceParameterTypeLink model) : base(container,model)
        {
        }

        public override bool Validate()
        {
            return Model.CustomDevice != null && Model.ParameterType != null && Model.Order!=0;
        }

        public string Name
        {
            get
            {
                return Model.Name;
            }
            set
            {
                Model.Name = value;
                OnPropertyChanged();
            }
        }

        public CustomDeviceViewModel Device
        {
            get { return Use<IPool>().GetOrCreateDBVM<CustomDeviceViewModel>(Model.CustomDevice); }
            set
            {
                value.LinkDeviceParam(Model);
                OnPropertyChanged();
            }
        }

        public ParameterTypeViewModel ParamType
        {
            get { return Use<IPool>().GetOrCreateDBVM<ParameterTypeViewModel>(Model.ParameterType); }
            set
            {
                value.LinkDeviceParam(Model);
                OnPropertyChanged();
            }
        }

        public override void Delete()
        {
            DeleteCommandParams();
            base.Delete();
        }

        private void DeleteCommandParams()
        {
            Use<IPool>().GetViewModels<CommandViewModel>().ForEach(a => a.DeleteDeviceParam(this.Model));
        }

        public IEnumerable<ParameterTypeViewModel> ParamTypes => Use<IPool>().GetViewModels<ParameterTypeViewModel>();

        public int Order
        {
            get { return Model.Order; } 
            set { Model.Order = value; }
        }

        public void LinkComandParam(ComandParameterLink model)
        {
            model.DeviceParameterTypeLink = Model;
        }
    }
}