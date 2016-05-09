using System.Net.Sockets;

namespace WebServer
{
    internal static class ThreadManager
    {
        public static void ClientThread(object stateInfo)
        {
            new ClientDispatcher((TcpClient)stateInfo);
        }
    }
}
