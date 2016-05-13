﻿using System;
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



    public interface ITreeNode
    {
        ITreeNode Parent { get; }
        IEnumerable<ITreeNode> Children { get; }
        string Name { get; }
        string Value { get; }
    }
}

