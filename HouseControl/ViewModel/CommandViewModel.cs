using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net.NetworkInformation;
using System.Runtime.Remoting.Metadata.W3cXsd2001;
using System.Text;
using System.Threading.Tasks;
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
            _comandEcexuted = null;
        }
        public override int Color { get; } = 3;
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
        private bool? _comandEcexuted;

        public override VMState VMState
        {
            get
            {
                return _comandEcexuted == null
                    ? VMState.Default
                    : _comandEcexuted.Value
                        ? VMState.Positive
                        : VMState.Negative;
            }
        }

        //private void ResetIsConnected()
        //{
            
        //    if (_comandEcexuted.HasValue && _comandEcexuted.Value)
        //    {
        //        _comandEcexuted = null;
        //        OnPropertyChanged(() => VMState);
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

            if (VMState == VMState.Negative)
                return;
            _comandEcexuted = false;
            while (!_comandEcexuted.Value)
            {
                var controller =
                    (LinkedObjectVm<Controller>) Use<IPool>()
                        .GetOrCreateDBVM<ControllerVM>(Model.CustomDevice.Controller);
                if (controller.VMState == VMState.Negative)
                {
                    _comandEcexuted = null;
                    return;
                }
                try
                {
                    await ExecuteInternal();
                }
                catch (Exception ex)
                {
                    await Task.Delay(100);
                }
            }
        }

        private async Task ExecuteInternal()
        {
            var sb = new StringBuilder();
            sb.Append(
                $"http://{Model.CustomDevice.Controller.IP}:{Model.CustomDevice.Controller.Port}/{Model.CustomDevice.CommandPath}");
            sb.Append($"?id={DeviceId}");
            Parameters.OrderBy(a => a.Order).ForEach(c => sb.Append($"&val{c.Order}={c.Parameter.Value}"));
            var http = sb.ToString();
            using (var res = Use<INetworkService>().AsyncRequest(http))
            {
                await res;
                if (string.IsNullOrEmpty(res.Result))
                    throw new Exception($"No Connect to {http}:\r\n{res.Result}");
            }
            _comandEcexuted = true;
        }

        public void DeleteDeviceParam(DeviceParameterTypeLink model)
        {
            Parameters.Where(a=>a.IsTypeOf(model)).ForEach(a=>a.Delete());
        }
    }
}