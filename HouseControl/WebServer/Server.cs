using Common;
using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace WebServer
{
    public class Server : ServiceBase
    {
        private TcpListener _listener;

        public void Start(int port)
        {
            var maxThreadsCount = Environment.ProcessorCount * 4;
            ThreadPool.SetMaxThreads(maxThreadsCount, maxThreadsCount);
            ThreadPool.SetMinThreads(1, 1);
            _listener = new TcpListener(IPAddress.Any, port);

            _listener.Start();

            while (true)
            {
                ThreadPool.QueueUserWorkItem(ClientDispatcher);
            }
        }

        public void Stop()
        {
            _listener?.Stop();
        }
        private static void ClientDispatcher(object tcpClient) //TODO ThreadPool.QueueUserWorkItem requires only static method to waitback??!
        {
            var client = tcpClient as TcpClient;
            if (client == null)
                return;

            int count;
            var request = "";
            var bytes = new byte[1024];

            while ((count = client.GetStream().Read(bytes, 0, bytes.Length)) > 0)
            {
                request += Encoding.ASCII.GetString(bytes, 0, count);

                if (request.IndexOf("\r\n\r\n", StringComparison.Ordinal) >= 0 || request.Length > 4096)
                {
                    break;
                }
            }

            const string Html = "<html><body><h1>It works!</h1></body></html>";

            var str = "HTTP/1.1 200 OK\nContent-type: text/html\nContent-Length:" + Html.Length + "\n\n" + Html;

            var buffer = Encoding.ASCII.GetBytes(str);

            client.GetStream().Write(buffer, 0, buffer.Length);

            client.Close();
        }
    }
}
