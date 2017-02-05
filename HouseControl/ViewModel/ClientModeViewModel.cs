using System;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Security.Policy;
using Facade;
using Model;
using ViewModelBase;

namespace ViewModel
{
    public class ClientModeViewModel : ViewModelBase.ViewModelBase
    {
        public ClientModeViewModel(IServiceContainer container) : base(container)
        {

        }
        public ModeProxy Mode { get; set; }
        public override int ID { get { return Mode.ID; }set {} }
        public string Name => Mode.Name;
        public bool IsSelected
        {
            get { return Mode.IsSelected; }
            set { SelectMode(value); }
        }

        private void SelectMode(bool value)
        {
            if (value)
            {
                Use<INetworkService>().SyncRequest(Url);
                Use<IPool>().GetViewModels<ClientModesViewModel>().Single().AskModes();
            }
        }

        public string Url
        {
            get
            {
                return
                    $"http://{Use<IPool>().GetViewModels<ClientOptionsViewModel>().Single().ServerIP}:{Use<IPool>().GetViewModels<ClientOptionsViewModel>().Single().ServerPort}/SetMode?{ID}";
            }
        }


        public void DeleteFromPool()
        {
            Use<IPool>().RemoveVM(typeof(ClientModeViewModel), ID);
        }
    }
}