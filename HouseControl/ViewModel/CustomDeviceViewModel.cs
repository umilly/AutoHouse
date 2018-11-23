using System;
using System.Collections.Generic;
using System.Linq;
using Facade;
using Model;
using ViewModelBase;

namespace ViewModel
{
    public class CustomDeviceViewModel : DeviceViewModelBase<CustomDevice>
    {
        public CustomDeviceViewModel(IServiceContainer container, CustomDevice model) : base(container, model)
        {
            if(IsFake)
                return;
        }

        public override void AddedToPool()
        {
            base.AddedToPool();
            if (Model != null && Model.Controller != null)
                OnControllerLinked();
        }

        public int NumberOnController { get;private set; }

        public override void LinklToParent(ITreeNode Parent)
        {
            throw new NotImplementedException();
        }

        public override void OnControllerLinked()
        {
            base.OnControllerLinked();
            NumberOnController = Model.Controller.CustomDevices.ToList().IndexOf(Model);
        }
    }

    public abstract class DeviceViewModelBase<T> :  LinkedObjectVm<T>, ICustomDevice where T:CustomDevice
    {
        
        public DeviceViewModelBase(IServiceContainer container, T model) : base(container,  model)
        {
            
        }

        public override bool Validate()
        {
            return Model.Controller != null&&!string.IsNullOrEmpty(Model.Name);
        }

        public override void LinklToParent(ITreeNode Parent)
        {
            throw new NotImplementedException();
        }

        public override Type ParentType { get { return typeof(ControllerVM); } }

        public override ITreeNode Parent => Controller;
        public override IEnumerable<ITreeNode> Children { get; }

        public ControllerVM Controller => Use<IPool>().GetOrCreateDBVM<ControllerVM>(this.Model.Controller);

        public override string Name
        {
            get { return Model.Name; }
            set
            {
                Model.Name = value; 
                OnPropertyChanged();
            }
        }

        public override string Value { get; set; }

        public string DeviceKey
        {
            get { return Model.CommandPath; }
            set
            {
                Model.CommandPath = value; 
                OnPropertyChanged();
            }
        }

        public IEnumerable<DeviceParamLinkVM> ParameterTypes
        {
            get
            {
                return Model.DeviceParameterTypeLinks
                    .Select(a => Use<IPool>().GetOrCreateDBVM<DeviceParamLinkVM>(a)).OrderBy(a=>a.Order);
            }
        }

        public void LinkTo(Controller model)
        {
            model.CustomDevices.Add(Model);
            Model.Controller = model;
            OnControllerLinked();
        }

        public virtual void OnControllerLinked()
        {
            
        }
        public void AddParam()
        {
            var pt = Use<IPool>().GetViewModels<ParameterTypeViewModel>().First();
            var link= Use<IPool>().CreateDBObject<DeviceParamLinkVM>();
            link.Order = ParameterTypes.Count();
            link.ParamType = pt;
            link.Name = "Параметр";
            link.Device = this as CustomDeviceViewModel;
            OnPropertyChanged(()=>ParameterTypes);
        }

        public void DeleteParams()
        {
            foreach (var source in ParameterTypes.ToList())
            {
                source.Delete();
            }
            OnPropertyChanged(() => ParameterTypes);
        }

        public override void Delete()
        {
            DeleteParams();
            base.Delete();
        }

        public void LinkToCommand(Command model)
        {
            model.CustomDevice = Model;
        }

        public void LinkDeviceParam(DeviceParameterTypeLink model)
        {
            model.CustomDevice = Model;
            Model.DeviceParameterTypeLinks.Add(model);
        }
    }
    public interface ICustomDevice : IViewModel
    {
        void LinkTo(Controller model);
        string Name { get; set; }
    }
}