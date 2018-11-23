using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using Facade;

namespace WebServer
{
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
                if(_request==null)
                    return;
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
                    var splited = item.Split('=');
                    if (splited.Length != 2)
                        return null;
                    var name = splited[0];
                    var val = splited[1];
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