using System;
using System.IO;
using System.Runtime.Serialization.Json;
using Facade;

namespace ViewModel
{
    public class ClientOptionsViewModel : ViewModelBase.ViewModelBase
    {
        private string _serverIp;
        private int _serverPort;

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

        public override int ID { get { return 1; } set {} }

        public string ServerIP
        {
            get { return _serverIp; }
            set
            {
                _serverIp = value; 
                OnPropertyChanged();
            }
        }

        public int ServerPort
        {
            get { return _serverPort; }
            set
            {
                _serverPort = value; 
                OnPropertyChanged();
            }
        }
    }
}