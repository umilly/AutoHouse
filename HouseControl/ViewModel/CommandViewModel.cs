using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net.NetworkInformation;
using System.Runtime.Remoting.Metadata.W3cXsd2001;
using System.Text;
using Facade;
using Model;
using ViewModel;
using ViewModelBase;

namespace ViewModel
{
    public class CommandViewModel : LinkedObjectVm<Command>
    {
        public CommandViewModel(IServiceContainer container,Command model)
            : base(container, model)
        {
            _isConnected = null;
        }

        public ReactionViewModel Reaction => Use<IPool>().GetOrCreateDBVM<ReactionViewModel>(Model.Reaction);

        public override void LinklToParent(ITreeNode newParent)
        {
            if (!(newParent is ReactionViewModel))
                throw new InvalidEnumArgumentException("comand's parent must ve scenario");
            (newParent as ReactionViewModel).LinkChildCommand(Model);
        }
        public override Type ParentType { get { return typeof(ReactionViewModel); } }

        public override ITreeNode Parent => Reaction;
        public override IEnumerable<ITreeNode> Children { get; }
        public override string Value { get; set; }
        
        public override bool Validate()
        {
            return !string.IsNullOrEmpty(Model.Name) && Reaction != null;
        }

        public override string Name
        {
            get { return Model.Name; }
            set
            {
                Model.Name = value;
                OnPropertyChanged();
            }
        }

        public IEnumerable<CustomDeviceViewModel> Devices
        {
            get {return Use<IPool>().GetViewModels<CustomDeviceViewModel>(); }
        }

        public CustomDeviceViewModel Device
        {
            get { return Use<IPool>().GetOrCreateDBVM<CustomDeviceViewModel>(Model.CustomDevice); }
            set
            {
                Parameters.ForEach(a=>a.Delete());
                value.LinkToCommand(Model);
                CreateCommandLinks();
                OnPropertyChanged();
            }
        }

        public override ITreeNode Copy()
        {
            var res=base.Copy() as CommandViewModel;
            res.CreateCommandLinks();
            return res;
        }

        public override void Delete()
        {
            Parameters.ForEach(a => a.Delete());
            base.Delete();
        }

        private void CreateCommandLinks()
        {
            foreach (var  devParam in Device.ParameterTypes.OrderBy(a=>a.Order))
            {
                var link=Use<IPool>().CreateDBObject<CommandParamLinkVm>();
                link.DeviceParamType = devParam;
                link.Command = this;
            }
            OnPropertyChanged(()=>Parameters);
        }

        public IEnumerable<CommandParamLinkVm> Parameters
        {
            get { return Model.ComandParameterLinks.Select(a => Use<IPool>().GetOrCreateDBVM<CommandParamLinkVm>(a)); }
        }

        public void LinkTo(Reaction model)
        {
            Model.Reaction = model;
        }

        public void LinkCommandParam(ComandParameterLink model)
        {
            model.Command = Model;
            Model.ComandParameterLinks.Add(model);
        }
        private bool? _isConnected;

        public override bool? IsConnected
        {
            get { return _isConnected; }
            set
            {
                _isConnected = value;
                if (value.HasValue && value.Value)
                {
                    UpdateStatus();
                }
                    //Use<ITimerSerivce>().Subscribe(this, ResetIsConnected, 1000);
                OnPropertyChanged(() => IsConnected);
            }
        }

        //private void ResetIsConnected()
        //{
            
        //    if (_isConnected.HasValue && _isConnected.Value)
        //    {
        //        _isConnected = null;
        //        OnPropertyChanged(() => IsConnected);
        //    }
        //}

        public int DeviceId
        {
            get
            {
                var dev = Use<IPool>().GetOrCreateDBVM<CustomDeviceViewModel>(Model.CustomDevice);
                return dev.NumberOnController;
            }
        }

        public async void Execute()
        {
            try
            {
                var sb = new StringBuilder();
                sb.Append(
                    $"http://{Model.CustomDevice.Controller.IP}:{Model.CustomDevice.Controller.Port}/{Model.CustomDevice.CommandPath}");
                sb.Append($"?id={DeviceId}");
                Parameters.OrderBy(a => a.Order).ForEach(c => sb.Append($"&val{c.Order}={c.Parameter.Value}"));
                IsConnected = false;
                var http = sb.ToString();
                using (var res = Use<INetworkService>().AsyncRequest(http))
                {
                    await res;
                    if (string.IsNullOrEmpty(res.Result))
                        throw new Exception($"No Connect to {http }:\r\n{res.Result}");
                }
                IsConnected = true;
            }
            catch (Exception ex)
            {
                IsConnected = false;
            }
        }

        public void DeleteDeviceParam(DeviceParameterTypeLink model)
        {
            Parameters.Where(a=>a.IsTypeOf(model)).ForEach(a=>a.Delete());
        }
    }
}