using System.Net.Sockets;
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
            

        }


        public void DeleteFromPool()
        {
            Use<IPool>().RemoveVM(typeof(ClientModeViewModel), ID);
        }
    }
}