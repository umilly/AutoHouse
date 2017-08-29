using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Facade;
using Model;
using ViewModelBase;

namespace ViewModel
{
    public class ModbusControllerViewModel : ControllerBase<ModBusController>
    { 
        public ModbusControllerViewModel(IServiceContainer container, Models dataBase, ModBusController controller) : base(container, dataBase, controller)
        {
            if(IsFake)
                return;
            Port = 0;
            IP = string.Empty;
        }

        public override Type ParentType { get { return null; } }

        public short? ComPort
        {
            get
            {
                return Model.ComPort;
            }
            set
            {
                Model.ComPort = value;
                OnPropertyChanged(()=>ComPort);
            }
        }

        public ComSpeed Speed
        {
            get { return (!Model.SpeedType.HasValue) ? ComSpeed.None : (ComSpeed) Model.SpeedType.Value; }
            set {
                switch (value)
                {
                    case ComSpeed.None:
                        Model.SpeedType = null;
                        break;
                    case ComSpeed.s9600:
                    case ComSpeed.s192000:
                        Model.SpeedType = (short) value;
                        break;
                    default:
                        throw new ArgumentOutOfRangeException(nameof(value), value, null);
                }
            }
        }

        protected override ICustomDevice CreateChildDev()
        {
            return Use<IPool>().CreateDBObject<ModBusDeviceViewModel>();
        }
    }

    public enum ComSpeed
    {
        None,
        s9600,
        s192000
    }
}
