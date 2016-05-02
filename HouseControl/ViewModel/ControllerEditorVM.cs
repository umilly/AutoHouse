using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Facade;
using Model;

namespace ViewModel
{
    public class ControllerEditorVM:ViewModelBase.EntytyObjectVM<Controller>
    {
        public ControllerEditorVM(IServiceContainer container) : base(container)
        {
        }

        public override int ID
        {
            get { return Model.Id; }
        }

        public string Name
        {
            get { return Model.Name; }
            set { Model.Name=value; }
        }

        public string IP
        {
            get { return Model.IP; }
            set { Model.IP=value; }
        }
        public int Port
        {
            get { return Model.Port; }
            set { Model.Port = value; }
        }

        protected override bool Validate()
        {
            return IP != null && Name != null;
        }

        public IEnumerable<Controller> GetControllers()
        {
            return Context.Controllers.ToList();
        }
    }
}
