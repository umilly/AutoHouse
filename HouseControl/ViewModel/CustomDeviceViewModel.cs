using System.Collections.Generic;
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
            set { Model.Name = value; }
        }

        public override string Value { get; set; }
        public override bool IsConnected { get; set; }

        public void LinkTo(Controller model)
        {
            Model.Controller = model;
        }
    }
}