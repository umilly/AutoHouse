﻿using Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Facade;
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
                return "Fail set param";
            }
        }

        public string SetMode(int modeId)
        {
            Use<IGlobalParams>().CurrentModeId = modeId;
            return "OK";
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

        public void Parse()
        {
            int count;
            var bytes = new byte[1024];
            while ((count = _client.GetStream().Read(bytes, 0, bytes.Length)) > 0)
            {
                _request += Encoding.ASCII.GetString(bytes, 0, count);
                if (_request.IndexOf("\r\n\r\n", StringComparison.Ordinal) >= 0 || _request.Length > 4096)
                {
                    break;
                }
            }
            var body = _request.Split('\r')[0].Split(' ')[1];
            var splitBody = body.Split('?','/');
            var command = splitBody[1];
            var comParams = new List<string>();
            for (int i = 2; i < splitBody.Length; i++)
            {
                comParams.Add(splitBody[i]);
            }
            _command = new WebCommand(command, comParams);
        }

        public void Response()
        {
            const string headerStr = "HTTP/1.1 200 OK\nContent-type: text/html;charset=utf-8\n";
            var body = $"<html lang=\"ru-RU\"><body><h1>{_command.Execute(_server)}</h1></body></html>";
            var res = $"{headerStr}\n\n{body}";
            var buffer = Encoding.UTF8.GetBytes(res);
            _client.GetStream().Write(buffer, 0, buffer.Length);
            _client.Close();
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
                _logger.Log(LogCategory.MobileWebServer, $"Client request handle error:\r\n{e.Message}");
            }
            finally
            {
                Dispose();
            }

        }

        public void Dispose()
        {
            //_client.Dispose();
        }
    }
}

public enum WebCommandType
{
    GetParams,
    GetModes,
    SetParam,
    SetMode
}

public class WebCommand
{
    public WebCommandType Type { get;private set; }
    public List<string> Params { get; private set; }

    public WebCommand(string type, List<string> @params)
    {

        Type = (WebCommandType)Enum.Parse(typeof(WebCommandType),type);
        Params = @params;
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
                return serv.SetParameter(int.Parse(Params[0]), Params[1]);
            case WebCommandType.SetMode:
                return serv.SetMode(int.Parse(Params[0]));
            default:
                throw new ArgumentOutOfRangeException();
        }
    }
}