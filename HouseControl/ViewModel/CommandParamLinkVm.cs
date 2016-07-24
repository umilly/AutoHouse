using System.Collections.Generic;
using System.Linq;
using Facade;
using Model;
using ViewModelBase;

namespace ViewModel
{
    public class CommandParamLinkVm : EntytyObjectVM<ComandParameterLink>
    {
        public CommandParamLinkVm(IServiceContainer container, Models dataBase, ComandParameterLink model) : base(container, dataBase, model)
        {
        }

        public override bool Validate()
        {
            return Model.Command != null && Model.DeviceParameterTypeLink != null && Model.Parameter != null;
        }

        public CommandViewModel Command
        {
            get { return Use<IPool>().GetDBVM<CommandViewModel>(Model.Command); }
            set
            {
                value.LinkCommandParam(Model);
                OnPropertyChanged();
            }
        }
        public IConditionSource Parameter
        {
            get { return Model.Parameter==null? EmptyValue.Instance : Use<IPool>().GetDBVM<ParameterViewModel>(Model.Parameter) as IConditionSource; }
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
            get { return Use<IPool>().GetDBVM<DeviceParamLinkVM>(Model.DeviceParameterTypeLink); }
            set
            {
                value.LinkComandParam(Model);
                OnPropertyChanged();
            }
        }
    }
}