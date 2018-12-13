using System;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Facade;
using Model;
using ViewModelBase;

namespace ViewModel
{
    public class ControllerVM : ControllerBase<Controller>
    {
        public ControllerVM(IServiceContainer container,Controller controller) : base(container,  controller)
        {
        }

        protected override Task OnCreate()
        {
            return base.OnCreate();
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

        public int ActivityTime
        {
            get { return Model.ActivityTime; }
            set
            {
                Model.ActivityTime = value;
                OnPropertyChanged();
            }
        }

        protected override ICustomDevice CreateChildDev()
        {
            return Use<IPool>().CreateDBObject<CustomDeviceViewModel>();
        }

        public void LinkCondition(Condition model)
        {
            model.Controller = Model;
        }
    }
}
