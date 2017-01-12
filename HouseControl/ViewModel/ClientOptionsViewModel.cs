using System;
using System.IO;
using System.Runtime.Serialization.Json;
using Facade;

namespace ViewModel
{
    public class ClientOptionsViewModel : ViewModelBase.ViewModelBase
    {
        public ClientOptionsViewModel(IServiceContainer container) : base(container)
        {

        }

        public void ReadOptions()
        {
            try
            {
                var clientSettings = Use<INetworkService>().Deserialize<ClientSettings>(File.ReadAllText("settings.ini"));
                ServerIP = clientSettings.ServerIP;
                ServerPort = clientSettings.ServerPort;
            }
            catch (Exception)
            {
                ServerIP = "127.0.0.1";
                ServerPort = 5555;
                SaveOptions();
            }
        }

        public void SaveOptions()
        {
            try
            {
                File.WriteAllText("settings.ini",
                    Use<INetworkService>().Serilize(new ClientSettings() {ServerPort = ServerPort, ServerIP = ServerIP}));
            }
            catch (Exception)
            {
                
            }
        }

        public override int ID { get; set; }

        public string ServerIP { get;private set; }
        public int ServerPort { get;private set; }


    }
}