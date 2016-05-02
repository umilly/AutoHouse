using System;
using System.Net.NetworkInformation;
using Facade;

namespace ViewModelBase
{
    public class NetworService : ServiceBase, INetworService
    {
        public string Ping(string address)
        {
            try
            {
                var p = new Ping();
                var reply = p.Send(address, 100);
                return reply.Status.ToString();
            }
            catch (Exception e)
            {

                if (e.InnerException != null)
                    return e.InnerException.Message;
                return e.Message;
            }

        }

    }
}