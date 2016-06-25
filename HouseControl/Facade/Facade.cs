using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Facade
{
    public interface IViewModel
    {
        int ID { get; }
    }


    public interface IView
    {
        IViewModel ViewModel { get; set; }
        Type VmType { get; }
        void OnClose();
    }

    public interface IWrapper
    {

    }

    public interface IViewService : IService
    {
        T CreateView<T>(int id = -1) where T : IView;
        IView CreateView(Type type, int Id = 0);
        IView NextView { get; set; }
        void ShowMessage(string res);
        T CreateView<T>(IViewModel viewModel) where T : IView;
        void FillTypes(Type[] types);
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

    public interface INetworkService : IService
    {
        string Ping(string address);
        string SyncRequest(string url);
        Task<string> AsyncRequest(string url);
    }

    public interface ILog : IService
    {
        void Log(LogCategory network, string message,bool showMesageBox=false);
    }

    public enum LogCategory
    {
        Network,
        Data
    }

    public interface IEntytyObjectVM:IViewModel
    {
        Type EntityType { get; }
        bool SavedInContext { get; set; }
        bool Validate();
        void SaveDB();
        void Delete();
    }
    public interface ITimerSerivce:IService {
        void Subsctibe(object key, Action action,int waitMilliSeconds,bool repeat=false);
        void UnSubsctibe(object key);
    }

    public interface ITreeNode : IViewModel
    {
        ITreeNode Parent { get; }
        IEnumerable<ITreeNode> Children { get; }
        string Name { get; }
        string Value { get; }
        bool IsConnected { get; }
        List<IContexMenuItem> ContextMenu { get; }
        void OnChildDelete(ITreeNode node);
    }

    public interface IContexMenuItem
    {
        string Text { get; }
        ICommand Todo { get; }
    }
}
