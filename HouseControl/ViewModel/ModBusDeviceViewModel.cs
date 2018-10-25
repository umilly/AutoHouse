using System;
using Facade;
using Model;

namespace ViewModel
{
    public class ModBusDeviceViewModel : DeviceViewModelBase<ModBusDevice>
    {
        public ModBusDeviceViewModel(IServiceContainer container, ModBusDevice model) : base(container,model)
        {
        }

        public override Type ParentType
        {
            get { return typeof(ModbusControllerViewModel); }
        }

        public ModbusDevType Type
        {
            get
            {
                return (!Model.IsCoil.HasValue)
                    ? ModbusDevType.None
                    : Model.IsCoil.Value ? ModbusDevType.Coil : ModbusDevType.Register;
            }
            set
            {
                switch (value)
                {
                    case ModbusDevType.None:
                        Model.IsCoil = null;
                        break;
                    case ModbusDevType.Coil:
                        Model.IsCoil = true;
                        break;
                    case ModbusDevType.Register:
                        Model.IsCoil = false;
                        break;
                    default:
                        throw new ArgumentOutOfRangeException(nameof(value), value, null);
                }
                OnPropertyChanged(() => Type);
            }
        }

        public short? Address
        {
            get { return Model.Address; }
            set { Model.Address = value; }
        }
    }

    public enum ModbusDevType
    {
        None,
        Coil,
        Register
    }
}