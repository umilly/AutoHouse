using System;
using System.ServiceProcess;
using WebServer;

namespace WebService
{
    public partial class WebService : ServiceBase
    {
        private Server _server;
        public WebService()
        {
            InitializeComponent();
            _server = new Server();
        }

        protected override void OnStart(string[] args)
        {
            try
            {
                _server.Start(80);//TODO configuration?
            }
            catch (Exception)
            {
                throw;
            }
        }

        protected override void OnStop()
        {
            try
            {
                _server.Stop();
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
