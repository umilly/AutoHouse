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

        protected override ICustomDevice CreateChildDev()
        {
            return Use<IPool>().CreateDBObject<CustomDeviceViewModel>();
        }
    }
}
