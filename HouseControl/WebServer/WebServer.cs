using Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
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
            _serverThread.Start();
            _listener.Start();
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
                Use<IPool>().SaveDB();
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
    }

    public class Client:IDisposable
    {
        private readonly TcpClient _client;
        private readonly IWebServer _server;
        private readonly ILog _logger;
        private const int timeout = 1000;
        private string _request;
        private WebCommand _command;
        public Client(TcpClient client, IWebServer server,ILog logger)
        {
            _client = client;
            _client.ReceiveTimeout = timeout;
            _client.SendTimeout = timeout;
            _server = server;
            _logger = logger;
        }

        private void Parse()
        {
            int count;
            var bytes = new byte[1024];
            if(_client==null)
                throw new ArgumentException("клиент не может быть null");
            //if (_client.Client.Blocking)
            //    throw new ArgumentException($"{_client.Client.RemoteEndPoint} сокет заблокирован");
            while ((count = _client.GetStream().Read(bytes, 0, bytes.Length)) > 0)
            {
                _request += Encoding.ASCII.GetString(bytes, 0, count);
                if (_request.IndexOf("\r\n\r\n", StringComparison.Ordinal) >= 0 || _request.Length > 4096)
                {
                    break;
                }
            }
            try
            {
                var body = _request.Split('\r')[0].Split(' ')[1];
                var splitBody = body.Split('?', '/');
                var command = splitBody[1];

                var comParams = ParseParamDict(splitBody.Count()>2?splitBody[2]:string.Empty);
                
                if (comParams != null)
                {
                    _command = new WebCommand(command, comParams);
                    return;
                }
                var comParamsOld = ParseParamList(splitBody);
                _command = new WebCommand(command, comParamsOld);
            }
            catch (Exception e)
            {
                _logger.Log(LogCategory.Network, $"Reqeuetst parse error: \r\n {_request} \r\n Url{((IPEndPoint)_client.Client.RemoteEndPoint).Address}");
                _logger.Log(LogCategory.Network, e);
            }
            
        }

        private List<string> ParseParamList(string[] splitBody)
        {
            var res = new List<string>();
            for (int i = 2; i < splitBody.Length; i++)
            {
                res.Add(splitBody[i]);
            }
            return res;
        }

        private Dictionary<string, string> ParseParamDict(string v)
        {
            try
            {
                var res = new Dictionary<string, string>();
                foreach (var item in v.Split('&'))
                {
                    var name = item.Split('=')[0];
                    var val = item.Split('=')[1];
                    res[name] = val;
                }
                return res;
            }
            catch(Exception e) {
                _logger.Log(LogCategory.Network, $"Canonic reqeuest  parse error: \r\n {_request} \r\n Url{((IPEndPoint)_client.Client.RemoteEndPoint).Address}");
                _logger.Log(LogCategory.Network, e);
            }
            return null;
        }

        private void Response()
        {
            if(_command==null)
                return;
            byte[] buffer = null;
            const string headerStr = "HTTP/1.1 200 OK\nContent-type: text/html;charset=utf-8\n";
            var body =
                _command.Json? _command.Execute(_server) :
                $"<html lang=\"ru-RU\"><body><h1>{_command.Execute(_server)}</h1></body></html>";
            var res = $"{headerStr}\n\n{body}";
            buffer = Encoding.UTF8.GetBytes(res);
            _client.GetStream().Write(buffer, 0, buffer.Length);
        }


        public async void Run()
        {
            try
            {
                await Task.Run(() => Parse());
                await Task.Run(() => Response());
            }
            catch (Exception e)
            {
                _logger.Log(LogCategory.MobileWebServer, $"Client request handle error:\r\n");
                _logger.Log(LogCategory.MobileWebServer, e);
            }
            finally
            {
                Dispose();
            }

        }

        public void Dispose()
        {
            _client.Close();
            _client.Dispose();
        }
    }
}



public class WebCommand
{
    public WebCommandType Type { get;private set; }
    public List<string> Params { get; private set; }
    public Dictionary<string, string> ParamDict { get; private set; }

    public WebCommand(string type, List<string> @params)
    {

        Type = (WebCommandType)Enum.Parse(typeof(WebCommandType),type);
        Params = @params;
    }
    public WebCommand(string type, Dictionary<string,string> @params)
    {

        Type = (WebCommandType)Enum.Parse(typeof(WebCommandType),type);
        ParamDict = @params;
    }

    public bool Json
    {
        get
        {
            switch (Type)
            {
                case WebCommandType.GetParams:
                case WebCommandType.GetModes:
                case WebCommandType.SetParam:
                case WebCommandType.SetMode:
                    return false;
                case WebCommandType.GetModesJson:
                case WebCommandType.GetParamsJson:
                    return true;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }

    public string Execute(IWebServer serv)
    {
        switch (Type)
        {
            case WebCommandType.GetParams:
                return serv.GetClientParams();
            case WebCommandType.GetModes:
                return serv.GetModes();
            case WebCommandType.SetParam:

                if(Params!=null)
                    return serv.SetParameter(int.Parse(Params[0]), Params[1]);
                if (ParamDict != null)
                    return serv.SetParameter(int.Parse(ParamDict["id"]), ParamDict["val"]);
                else
                    throw new ArgumentException($"Параметры команыды{Type}указаны не верно");
                break;
            case WebCommandType.SetMode:
                return serv.SetMode(int.Parse(Params[0]));
            case WebCommandType.GetModesJson:
                return serv.GetModesJson();
            case WebCommandType.GetParamsJson:
                return serv.GetParametersJson();
            default:
                throw new ArgumentOutOfRangeException();
        }
    }
}