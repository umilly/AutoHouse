using Common;
using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace WebServer
{
    public class Server : ServiceBase
    {
        readonly TcpListener _listener;

        private Server(int port)
        {
            var maxThreadsCount = Environment.ProcessorCount * 4;
            ThreadPool.SetMaxThreads(maxThreadsCount, maxThreadsCount);
            ThreadPool.SetMinThreads(1, 1);

            _listener = new TcpListener(IPAddress.Any, port);
            _listener.Start();

            while (true)
            {
                ThreadPool.QueueUserWorkItem(new WaitCallback(ThreadManager.ClientThread), _listener.AcceptTcpClient());
            }
        }

        ~Server()
        {
            _listener?.Stop();
        }

        static void Main(string[] args)
        {
            new Server(63);
        }
    }
}
