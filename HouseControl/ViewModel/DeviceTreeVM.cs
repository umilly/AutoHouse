using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Facade;
using ViewModelBase;

namespace ViewModel
{
    public class DeviceTreeVM:ViewModelBase.ViewModelBase
    {
        private ITreeNode[] _devices;

        public DeviceTreeVM(IServiceContainer container) : base(container)
        {
        }

        public ITreeNode[] Devices
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
}

