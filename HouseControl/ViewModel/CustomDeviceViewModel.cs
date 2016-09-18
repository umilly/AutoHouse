using System;
using System.Collections.Generic;
using System.Linq;
using Facade;
using Model;
using ViewModelBase;

namespace ViewModel
{
    public class CustomDeviceViewModel : LinkedObjectVM<CustomDevice>
    {
        
        public CustomDeviceViewModel(IServiceContainer container, Models dataBase, CustomDevice model) : base(container, dataBase, model)
        {
            
        }

        public override bool Validate()
        {
            return Model.Controller != null&&!string.IsNullOrEmpty(Model.Name);
        }

        public override ITreeNode Parent => Controller;
        public override IEnumerable<ITreeNode> Children { get; }

        public ControllerVM Controller => Use<IPool>().GetDBVM<ControllerVM>(this.Model.Controller);

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

        
        public override bool? IsConnected
        {
            get { return null; }
            set
            {
            }
        }


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
                    .Select(a => Use<IPool>().GetDBVM<DeviceParamLinkVM>(a)).OrderBy(a=>a.Order);
            }
        }

        public void LinkTo(Controller model)
        {
            Model.Controller = model;
        }

        public void AddParam()
        {
            var pt = Use<IPool>().GetViewModels<ParameterTypeViewModel>().First();
            var link= Use<IPool>().CreateDBObject<DeviceParamLinkVM>();
            link.Order = ParameterTypes.Count();
            link.ParamType = pt;
            link.Name = "Параметр";
            link.Device = this;
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
}