using System.Collections.Generic;
using System.Linq;
using Facade;
using Model;
using ViewModelBase;

namespace ViewModel
{
    public class CommandParamLinkVm : EntityObjectVm<ComandParameterLink>
    {
        public CommandParamLinkVm(IServiceContainer container,  ComandParameterLink model) : base(container,  model)
        {
        }

        public override bool Validate()
        {
            return Model.Command != null && Model.DeviceParameterTypeLink != null && Model.Parameter != null;
        }

        public CommandViewModel Command
        {
            get { return Use<IPool>().GetOrCreateDBVM<CommandViewModel>(Model.Command); }
            set
            {
                value.LinkCommandParam(Model);
                OnPropertyChanged();
            }
        }

        public override void Delete()
        {
            Model.Command?.ComandParameterLinks?.Remove(Model);
            Model.Parameter?.ComandParameterLinks?.Remove(Model);
            base.Delete();
        }

        public IConditionSource Parameter
        {
            get { return Model.Parameter==null? EmptyValue.Instance : Use<IPool>().GetOrCreateDBVM<ParameterViewModel>(Model.Parameter) as IConditionSource; }
            set
            {
                if (value == EmptyValue.Instance)
                {
                    Model.Parameter = null;
                    return;
                }
                var val = value as ParameterViewModel;
                val.LinkDeviceParam(Model);
                OnPropertyChanged();
            }
        }

        public IEnumerable<IConditionSource> AvailableParameters
        {
            get
            {
                return
                    Use<IPool>()
                        .GetViewModels<ParameterViewModel>()
                        .Where(a => a.ParamType == DeviceParamType.ParamType)
                        .Cast<IConditionSource>()
                        .Union(new[] { EmptyValue.Instance });
            }
        }

        public DeviceParamLinkVM DeviceParamType
        {
            get { return Use<IPool>().GetOrCreateDBVM<DeviceParamLinkVM>(Model.DeviceParameterTypeLink); }
            set
            {
                value.LinkComandParam(Model);
                OnPropertyChanged();
            }
        }

        public int Order => Model.DeviceParameterTypeLink.Order;

        public bool IsTypeOf(DeviceParameterTypeLink model)
        {
            return Model.DeviceParameterTypeLink == model;
        }
    }
}