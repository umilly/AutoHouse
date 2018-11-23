using Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using Facade;
using Model;
using ViewModel;
using ViewModelBase;

namespace WebServer
{
    public class WebServer : ServiceBase, IWebServer
    {
        private TcpListener _listener;
        private Thread _serverThread;
        public override void OnContainerSet()
        {
            base.OnContainerSet();
            _listener = new TcpListener(IPAddress.Any, 5555);
            _serverThread = new Thread(Loop);
            var maxThreadsCount = Environment.ProcessorCount * 4;
            ThreadPool.SetMaxThreads(maxThreadsCount, maxThreadsCount);
            ThreadPool.SetMinThreads(2, 2);
        }

        private async void Loop()
        {
            while (true)
            {
                var task = await _listener.AcceptTcpClientAsync();
                var client = new Client(task, this, Use<ILog>());
                client.Run();
            }
        }

        public void Start()
        {
            _listener.Start();
            _serverThread.Start();
        }

        public void Stop()
        {
            _listener?.Stop();
            _serverThread?.Abort();
        }

        public string GetClientParams()
        {
            var sb = new StringBuilder();
            Use<IPool>().GetViewModels<ParameterViewModel>().Where(a => a.IsPublic).ForEach(a => sb.Append($"<br>{a.ID.ToString()}:{a.Name}={a.Value}<br>"));
            return sb.ToString();
        }

        public string GetModes()
        {
            var sb = new StringBuilder();
            Use<IPool>().GetViewModels<ModeViewModel>().ForEach(a => sb.Append($"<br>{a.ID.ToString()}:{a.Name}={a.Value}<br>"));
            return sb.ToString();

        }

        public string SetParameter(int paramId, string value)
        {
            try
            {
                Use<IPool>().GetViewModels<ParameterViewModel>().First(a => a.ID == paramId).Value = value;
                return "OK";
            }
            catch (Exception e)
            {
                var prefix = "Fail set param";
                Use<ILog>().Log(LogCategory.Data, prefix);
                Use<ILog>().Log(LogCategory.Data, e);
                return prefix;
            }
        }

        public string SetMode(int modeId)
        {
            foreach (var mode in Use<IPool>().GetViewModels<ModeViewModel>())
            {
                mode.IsSelected = mode.ID == modeId;
            }
            return "OK";
        }

        public string GetModesJson()
        {
            var res = new Modes {ModeList = new List<ModeProxy>()};
            foreach (var modeViewModel in Use<IPool>().GetViewModels<ModeViewModel>())
            {
                res.ModeList.Add(modeViewModel.GetProxy);
            }
            return Use<INetworkService>().Serilize(res);
        }
        public string GetParametersJson()
        {
            var res = new Parameters() { ParamList = new List<ParameterProxy>() };
            foreach (var modeViewModel in Use<IPool>().GetViewModels<ParameterViewModel>().Where(a=>a.IsPublic))
            {
                res.ParamList.Add(modeViewModel.GetProxy());
            }
            return Use<INetworkService>().Serilize(res);
        }

        private Dictionary<string,ControllerVM> _controllerCache=new Dictionary<string, ControllerVM>();
        public string SetSensorsValues(string ip, Dictionary<string, string> sensorValues)
        {

            if (!_controllerCache.ContainsKey(ip))
            {
                _controllerCache[ip]= Use<IPool>().GetViewModels<ControllerVM>().First(a => a.IP == ip);
            }
            _controllerCache[ip].SetSensorsValues(sensorValues);
            return "OK";
        }
    }
}