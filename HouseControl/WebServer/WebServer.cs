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
    public class WebServer : ServiceBase,IWebServer
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
            var sb=new StringBuilder();
            Use<IPool>().GetViewModels<ParameterViewModel>().Where(a => a.IsPublic).ForEach(a=>sb.Append($"<br>{a.Name}={a.Value}<br>"));
            return sb.ToString();
        }
    }

    public class Client
    {
        private readonly TcpClient _client;
        private readonly IWebServer _server;
        private string _request;
        public Client(TcpClient client,IWebServer server)
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
            var clientParams = _server.GetClientParams();
            byte[] htmlstart = Encoding.ASCII.GetBytes($"<html lang=\"ru-RU\"><body><h1>");
            byte[] htmlend = Encoding.ASCII.GetBytes($"</h1></body></html>");
            byte[] body = Encoding.UTF8.GetBytes(clientParams);
            var responseLen = htmlstart.Length + htmlend.Length + body.Length;
            byte[] header =  Encoding.ASCII.GetBytes("HTTP/1.1 200 OK\nContent-type: text/html;charset=utf-8\nContent-Length:" + responseLen+"\n\n");
            var buffer = new byte[responseLen+ header.Length];
            Array.Copy(header,0,buffer, 0,header.Length);
            Array.Copy(htmlstart, 0, buffer, header.Length, htmlstart.Length);
            Array.Copy(body, 0, buffer, header.Length+ htmlstart.Length, body.Length);
            Array.Copy(htmlend, 0, buffer, header.Length + htmlstart.Length+body.Length, htmlend.Length);

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
