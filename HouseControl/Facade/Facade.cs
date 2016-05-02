using System;
using System.ComponentModel.Design;
using System.Threading.Tasks;

namespace Facade
{
    public interface IViewModel
    {
        int ID { get; }
        void OnCreate(int id);
    }


    public interface IView
    {
        IViewModel ViewModel { get; set; }
        Type VmType { get; }
    }

    public interface IWrapper
    {

    }

    public interface IViewService : IService
    {
        T CreateView<T>(int id = 0) where T : IView;
        IView CreateView(Type type, int Id = 0);
        IView NextView { get; set; }
        void ShowMessage(string res);
    }

    public interface IService
    {
        void SetContainer(IServiceContainer container);
    }

    public interface IServiceContainer
    {
        void RegisterType<TInterface, TImplementation>();

        T Use<T>() where T : class;

        void InjectServicesToCustomObject(object customObject);

        void UnRegister<T>(bool skipDispose = false) where T : class;

        string GetServiceNames();

        void UnRegisterAll();
    }

    public interface IHaveID
    {
        int ID { get; }
    }

    public interface INetworService : IService
    {
        string Ping(string address);
        string SyncRequest(string url);
        Task<string> AsyncRequest(string url);
    }

    public interface ILog:IService
    {
        void Log(LogCategory network, string message);
    }

    public enum LogCategory
    {
        Network,
        Data
    }
}
