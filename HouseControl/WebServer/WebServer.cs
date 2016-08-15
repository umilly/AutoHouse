using Common;
using System;
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
                var client = new Client(task, this);
                try
                {
                    client.Run();
                }
                catch (Exception e)
                {
                    Use<ILog>().Log(LogCategory.Network, e.ToString());
                }
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
            Use<IPool>().GetViewModels<ParameterViewModel>().Where(a => a.IsPublic).ForEach(a => sb.Append($"<br>{a.Name}={a.Value}<br>"));
            return sb.ToString();
        }
    }

    public class Client
    {
        private readonly TcpClient _client;
        private readonly IWebServer _server;
        private string _request;
        public Client(TcpClient client, IWebServer server)
        {
            _client = client;
            _server = server;
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

        }

        public void Response()
        {
            const string headerStr = "HTTP/1.1 200 OK\nContent-type: text/html;charset=utf-8\n";
            var clientParams = _server.GetClientParams();
            var body = $"<html lang=\"ru-RU\"><body><h1>{clientParams}</h1></body></html>";
            var res = $"{headerStr}\n\n{body}";
            var buffer = Encoding.UTF8.GetBytes(res);
            _client.GetStream().Write(buffer, 0, buffer.Length);
            _client.Close();
        }


        public async void Run()
        {
            await Task.Run(() => Parse());
            await Task.Run(() => Response());
        }
    }
}
