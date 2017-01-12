﻿using System;
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
        public ClientMainViewModel(IServiceContainer container) : base(container)
        {
            container.RegisterType<IPool, VMFactory>();
            container.RegisterType<INetworkService, NetworkService>();
            container.RegisterType<ILog, EventLogger>();
            container.RegisterType<ITimerSerivce, TimerService>();
            container.RegisterType<IGlobalParams, GlobalParams>();
            Use<ITimerSerivce>().Subsctibe(this, UpdateParametersValues, 500,true);
        }

        private void UpdateParametersValues()
        {
            
        }

        public override int ID
        {
            get { return 1; }
            set { }
        }
        
        public void InitSettings()
        {
            var settings = Use<IPool>().GetOrCreateVM<ClientOptionsViewModel>(1);
            settings.ReadOptions();
        }
    }
}
