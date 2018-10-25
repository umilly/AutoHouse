using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using Facade;
using Model;
using ViewModelBase;

namespace ViewModel
{
    public class SettingsVM : ViewModelBase.ViewModelBase
    {
        private long _relayCount;
        private bool _isDebug;
        public ObservableCollection<RelayViewModel> Relays { get; private set; }
        

        public SettingsVM(IServiceContainer container) : base(container)
        {
            Relays=new ObservableCollection<RelayViewModel>();
        }

        public long RelayCount
        {
            get { return _relayCount; }
            set
            {
                _relayCount = value;
                OnRelayChanged();
                OnPropertyChanged();
            }
        }

        public override int ID
        {
            get { return 1; }
            set { }
        }

        public bool IsDebug
        {
            get { return _isDebug; }
            set
            {
                _isDebug = value; 
                OnPropertyChanged();
            }
        }

        private void OnRelayChanged()
        {
            var relaysCount = Use<IPool>().GetViewModels<RelayViewModel>().Count();
            int difference = (int)RelayCount - relaysCount;
            if (difference > 0)
            {
                for (int i = 0; i < difference; i++)
                {
                    var number = relaysCount + i + 1;
                    var vm = Use<IPool>().GetOrCreateVM<RelayViewModel>(number);
                    vm.RelayData=
                    new RelayData()
                    {
                        Address = "192.168.1."+(201+ Relays.Count),
                        Number = number,
                        StartCommand = "startrele" + number,
                        StopCommand = "stoprele" + number,
                        Name = "Реле "+ number
                    };
                    vm.UpdateIsAvailable();
                    Relays.Add(vm);
                }
            }
            if (difference < 0)
            {
                for (int i = relaysCount - 1; i >= relaysCount+ difference; i--)
                {
                    var number =i + 1;
                    Use<IPool>().RemoveVM<RelayViewModel>(number);
                    Relays.RemoveAt(i);
                }
                
            }
            OnPropertyChanged("Relays");
        }

        public void Apply(Settings settings2)
        {
            //Relays.Clear();
            //foreach (var relayData in settings2.Relays)
            //{
            //    var vm= Use<IPool>().GetOrCreateVM<RelayViewModel>(relayData.Number);
            //    vm.RelayData = relayData;
            //    vm.UpdateIsAvailable();
            //    Relays.Add(vm);
            //}

            _relayCount = 0;//settings2.Count;
            IsDebug = true;
        }
    }
}
