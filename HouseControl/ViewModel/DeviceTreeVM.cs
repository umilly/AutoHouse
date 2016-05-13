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
        public DeviceTreeVM(IServiceContainer container) : base(container)
        {
        }

        public ITreeNode[] Devices { get; set; }
        public override void OnCreate(int id)
        {
            base.OnCreate(id);
            var allDevs = Use<IPool>().GetViewModels<IDevice>();

        }

        public override int ID { get; set; }
    }

    public interface IDevice:IViewModel
    {
    }

    public interface ITreeNode
    {
        ITreeNode Parent { get; }
        ITreeNode Children { get; }
        string Name { get; }
        string Value { get; }
    }
}

