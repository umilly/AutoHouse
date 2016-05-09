using System.Net.Sockets;
using System.Text;

namespace WebServer
{
    class ClientDispatcher
    {
        public ClientDispatcher(TcpClient client)
        {
            string Request = "";

            byte[] Buffer = new byte[1024];

            int Count;

            while ((Count = client.GetStream().Read(Buffer, 0, Buffer.Length)) > 0)
            {
                Request += Encoding.ASCII.GetString(Buffer, 0, Count);

                if (Request.IndexOf("\r\n\r\n") >= 0 || Request.Length > 4096)
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
