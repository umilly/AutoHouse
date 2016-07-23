using System.Collections.Generic;
using System.Linq;
using Facade;
using Model;
using ViewModelBase;

namespace ViewModel
{
    public class CustomDeviceViewModel : LinkedObjectVM<CustomDevice>
    {
        public CustomDeviceViewModel(IServiceContainer container, Models dataBase, CustomDevice model) : base(container, dataBase, model)
        {
        }

        public override bool Validate()
        {
            return Model.Controller != null&&!string.IsNullOrEmpty(Model.Name);
        }

        public override ITreeNode Parent => Controller;
        public override IEnumerable<ITreeNode> Children { get; }

        public ControllerVM Controller => Use<IPool>().GetDBVM<ControllerVM>(this.Model.Controller);

        public override string Name
        {
            get { return Model.Name; }
            set
            {
                Model.Name = value; 
                OnPropertyChanged();
            }
        }

        public override string Value { get; set; }
        public override bool IsConnected { get; set; }

        public IEnumerable<ParameterTypeViewModel> ParameterTypes
        {
            get
            {
                return Model.ParameterTypes
                    .Select(parameterType => Use<IPool>().GetDBVM<ParameterTypeViewModel>(parameterType));
            }
        }

        public void LinkTo(Controller model)
        {
            Model.Controller = model;
        }

        public void AddParam(ParameterTypeValue parameterTypeValue)
        {
            var pt = Context.ParameterTypes.Find((int)parameterTypeValue);
            Model.ParameterTypes.Add(pt);
            OnPropertyChanged(()=>ParameterTypes);
        }

        public void DeleteParams()
        {
            Model.ParameterTypes.Clear();
        }

        public void LinkToCommand(Command model)
        {
            model.CustomDevice = Model;
        }
    }
}