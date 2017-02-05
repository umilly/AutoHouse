using System;
using System.Collections.Generic;
using System.Linq;
using Facade;
using Model;
using ViewModelBase;

namespace ViewModel
{
    public class ClientModesViewModel : ViewModelBase.ViewModelBase
    {
        public ClientModesViewModel(IServiceContainer container) : base(container)
        {
          
        }

        public string ImagePath = "D:\\house.jpg";
        public override int ID{get { return -1; }
            set { }
        }

        public IEnumerable<ClientModeViewModel> Modes => Use<IPool>().GetViewModels<ClientModeViewModel>();

        public string ServerUrl
        {
            get
            {
                var options = Use<IPool>().GetViewModels<ClientOptionsViewModel>().Single();
                return $"http://{options.ServerIP}:{options.ServerPort}/{WebCommandType.GetModesJson}?{DateTime.Now.Ticks}";
            }
        }

        public async void AskModes()
        {
            var modes = await Use<INetworkService>().AsyncRequest(ServerUrl);
            Use<IPool>().GetViewModels<ClientModeViewModel>().ToList().ForEach(a => a.DeleteFromPool());
            if (string.IsNullOrEmpty(modes) )
                return;
            var res=Use<INetworkService>().Deserialize<Modes>(modes);
            foreach (var modeProxy in res.ModeList)
            {
                var mode= Use<IPool>().CreateVM<ClientModeViewModel>();
                mode.Mode = modeProxy;
            }
            OnPropertyChanged(()=> Modes);
        }
    }
}