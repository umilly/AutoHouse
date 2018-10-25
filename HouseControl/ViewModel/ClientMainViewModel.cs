using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using Common;
using Facade;
using Model;
using ViewModelBase;

namespace ViewModel
{
    public class ClientMainViewModel : ViewModelBase.ViewModelBase
    {
        private Page _page;

        public ClientMainViewModel(IServiceContainer container) : base(container)
        {
            container.RegisterType<IPool, VMFactory>();
            container.RegisterType<INetworkService, NetworkService>();
            container.RegisterType<ILog, EventLogger>();
            container.RegisterType<ITimerSerivce, TimerService>();
            container.RegisterType<IGlobalParams, GlobalParams>();
            Use<ITimerSerivce>().Subscribe(this, UpdateParametersValues, 500,true);
        }

        private void UpdateParametersValues()
        {
            
        }

        public override int ID
        {
            get { return 1; }
            set { }
        }

        public string Mode
        {
            get
            {
                switch (Page)
                {
                    case Page.Main:
                        return "Главная";
                        break;
                    case Page.Climax:
                        return "Климат";
                        break;
                    case Page.Light:
                        return "Свет";
                        break;
                    case Page.Remote:
                        return "Оборудование";
                        break;
                    case Page.Cameras:
                        return "Камеры";
                        break;
                    case Page.Access:
                        return "Охрана";
                        break;
                    case Page.Settings:
                        return "Настройки";
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }

        public Page Page
        {
            get { return _page; }
            set
            {
                _page = value; 
                OnPropertyChanged(()=>Mode);
                OnPropertyChanged(() => BGVisible);
            }
        }

        public bool BGVisible => _page == Page.Main;
        public void InitSettings()
        {
            var settings = Use<IPool>().GetOrCreateVM<ClientOptionsViewModel>(1);
            settings.ReadOptions();
        }
    }
}
