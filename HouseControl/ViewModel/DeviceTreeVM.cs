using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Facade;
using ViewModelBase;

namespace ViewModel
{
    public class DeviceTreeVM:ViewModelBase.ViewModelBase
    {
        private IDeviceTreeNode[] _devices;

        public DeviceTreeVM(IServiceContainer container) : base(container)
        {
        }

        public IDeviceTreeNode[] Devices
        {
            get { return _devices; }
            set
            {
                _devices = value;
                OnPropertyChanged();
            }
        }

        public override int ID { get; set; }
    }



    public interface IDeviceTreeNode
    {
        IDeviceTreeNode Parent { get; }
        IEnumerable<IDeviceTreeNode> Children { get; }
        string Name { get; }
        string Value { get; }
        bool IsConnected { get; }
    }
}

