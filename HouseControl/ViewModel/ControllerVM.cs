using System;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Facade;
using Model;
using ViewModelBase;

namespace ViewModel
{
    public class ControllerVM : ControllerBase<Controller>
    {
        public ControllerVM(IServiceContainer container, Models dataBase, Controller controller) : base(container, dataBase, controller)
        {
        }

        public override void OnChildDelete(ITreeNode node)
        {
            base.OnChildDelete(node);
            foreach (var customDeviceViewModel in Children.OfType<CustomDeviceViewModel>())
            {
                customDeviceViewModel.OnControllerLinked();
            }
        }

        public override Type ParentType { get { return null; } }

        protected override ICustomDevice CreateChildDev()
        {
            return Use<IPool>().CreateDBObject<CustomDeviceViewModel>();
        }
    }
}
