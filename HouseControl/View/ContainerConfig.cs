using System.Linq;
using Common;
using Facade;
using Model;
using ViewModel;
using ViewModelBase;
using VMBase;

namespace WpfApplication
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        public void InitContainer()
        {
            _container.RegisterType<IViewService, ViewService>();
            _container.RegisterType<IWebServer, WebServer.WebServer>();
            var types = GetType().Assembly.GetTypes().Where(type => typeof(IView).IsAssignableFrom(type));
            _container.Use<IViewService>().FillTypes(types.ToArray());
            _container.RegisterType<ISettings, Settings>();
            _container.RegisterType<IContext, DataContext<Models>>();
            _container.RegisterType<ILog, EventLogger>();
            _container.RegisterType<IPool, VMFactory>();
            _container.RegisterType<ICopyService, CopyService>();
            _container.RegisterType<INetworkService, NetworkService>();
            _container.RegisterType<ITimerSerivce, TimerService>();
            _container.RegisterType<IReactionService, ReactionService>();
            _container.RegisterType<IGlobalParams, GlobalParams>();
            _container.Use<IPool>().InitByAssambly(new[] { typeof(MainViewModel).Assembly, typeof(ViewModelBase.ViewModelBase).Assembly });
        }
    }
}