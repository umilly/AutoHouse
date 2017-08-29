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
    public class MainViewModel : ViewModelBase.ViewModelBase
    {
        public MainViewModel(IServiceContainer container) : base(container)
        {
            container.RegisterType<ILog, EventLogger>();
            container.RegisterType<IPool, VMFactory>();
            container.Use<IPool>().Init();
            container.RegisterType<ICopyService, CopyService>();
            container.RegisterType<INetworkService, NetworkService>();
            container.RegisterType<ITimerSerivce, TimerService>();
            container.RegisterType<IReactionService, ReactionService>();
            container.RegisterType<IGlobalParams, GlobalParams>();
            PreparePool();
            Use<ITimerSerivce>().Subsctibe(this, UpdateControllers, 500,true);
            Use<IWebServer>().Start();
        }

        private void UpdateControllers()
        {
            Use<IPool>().GetViewModels<ControllerVM>().ForEach(a=>a.Update());
            Use<IPool>().GetViewModels<SensorViewModel>().ForEach(a => a.UpdateValue());
            Use<IReactionService>().Check();
        }

        private void PreparePool()
        {
            var types = GetType().Assembly.GetTypes().Where(type => typeof (IEntytyObjectVM).IsAssignableFrom(type)&&!type.IsAbstract);
            Use<IPool>().FillPool(types.ToArray());
        }

        public override int ID
        {
            get { return 1; }
            set { }
        }
        
        public void InitSettings()
        {
            var settings = Use<IPool>().GetOrCreateVM<SettingsVM>(1);
            try
            {
                var serializer=new DataContractJsonSerializer(typeof(Settings));
                var stream = new FileStream("settings.ini", FileMode.Open);
                var settings2 =(Settings)  serializer.ReadObject(stream);
                stream.Close();
                stream.Dispose();
                settings.Apply(settings2);
            }
            catch (Exception)
            {
                settings.RelayCount = 13;
            }
        }

        public void SaveSettings()
        {
            Use<IPool>().SaveDB();
            //var serializer = new DataContractJsonSerializer(typeof(Settings));
            //File.Delete("settings.ini");
            //var settings = Use<IPool>().GetOrCreateVM<SettingsVM>(1);
            //var serSets = new Settings() {Relays = settings.Relays.Select(a=>a.RelayData).ToList(),Count = settings.RelayCount,IsDebug = settings.IsDebug};
            //var fileStream = new FileStream("settings.ini", FileMode.CreateNew);
            //serializer.WriteObject(fileStream, serSets);
            //fileStream.Close();
            //fileStream.Dispose();
        }
    }
}
