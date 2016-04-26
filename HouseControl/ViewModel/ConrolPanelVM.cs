using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Facade;
using Model;
using ViewModelBase;

namespace ViewModel
{
    public class ConrolPanelVM : ViewModelBase.ViewModelBase
    {
        public ObservableCollection<RelayViewModel> Relays { get; set; }

        public ConrolPanelVM(IServiceContainer container) : base(container)
        {
            
        }

        public void FillRelays()
        {
            var relays = Use<IPool>().GetViewModels<RelayViewModel>();
            if (Relays == null)
            {
                Relays = new ObservableCollection<RelayViewModel>();
                OnPropertyChanged("Relays");
            }
            Relays.Clear();
            relays.ForEach(Relays.Add);
        }



        public override int ID
        {
            get { return 1; }
        }

        public void SwitchAll(bool b)
        {
            foreach (var relayViewModel in Relays)
            {
                relayViewModel.Switch(b);
            }
        }
    }
}