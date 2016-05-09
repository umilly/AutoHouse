using Common;
using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace WebServer
{
    public class Server : ServiceBase
    {
        private Server(int port)
        {
            var maxThreadsCount = Environment.ProcessorCount * 4;
            ThreadPool.SetMaxThreads(maxThreadsCount, maxThreadsCount);
            ThreadPool.SetMinThreads(1, 1);
            var listener = new TcpListener(IPAddress.Any, port);

            listener.Start();

            while (true)
            {
                ThreadPool.QueueUserWorkItem(ThreadManager.ClientThread, listener.AcceptTcpClient());
            }
        }

        static void Main(string[] args)
        {
            int port;
            if (int.TryParse(args[0], out port))
            {
                var server = new Server(port);
            }
        }
    }
}
