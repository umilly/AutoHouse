using System;
using System.Collections.Generic;
using System.IO;
using System.Net.NetworkInformation;
using System.Security.Cryptography.X509Certificates;
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
        void OnVMSet();
    }

    public interface IWrapper
    {

    }

    public interface IWebServer : IService
    {
        void Start();
        void Stop();
        string GetClientParams();
        string GetModes();
        string SetParameter(int paramId, string value);
        string SetMode(int modeId);
        string GetModesJson();
        string GetParametersJson();
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
    public interface IDBModel
    {
    }
    public interface IHaveID: IDBModel
    {
        int ID { get; }
    }

    public interface INetworkService : IService
    {
        IPStatus Ping(string address);
        string SyncRequest(string url);
        Task<string> AsyncRequest(string url);
        T Deserialize<T>(string json);
        string Serilize<T>(T obj);
    }

    public interface ILog : IService
    {
        void Log(LogCategory network, string message,bool showMesageBox=false);
        void Log(LogCategory network, Exception e, bool showMesageBox = false);
        void LogNetException(Exception e, string prefix);
    }

    public enum LogCategory
    {
        Network,
        Data,
        Configuration,
        MobileWebServer,
        Command,
        Debug
    }

    public interface IEntityObjectVM : IViewModel
    {
        bool IsModelActual { get; }
        void AddedToPool();
        bool CompareModel(IHaveID id);
        void Delete();
    }
    public interface IEntityObjectVM<out T> : IEntityObjectVM where T : class, IDBModel
    {
        T Model { get; }
    }
    public interface ITimerSerivce : IDisposable,IService
    {
        void Reset();
        void Subscribe(object key, Action action, int waitMilliSeconds, bool repeat = false);
        void UnSubsctibe(object key);
    }
    public interface IEvent
    {
    }
    public interface IEventDispatcher
    {
        void Publish<T>(T e) where T : IEvent;
        void Subscribe<T>(object o, Action<T> action) where T : IEvent;
        void UnSubscribe(object o);
    }
    public interface ITreeNode : IViewModel
    {
        ITreeNode Parent { get; }
        IEnumerable<ITreeNode> Children { get; }
        string Name { get; }
        string Value { get; }
        bool? IsConnected { get; }
        int LastUpdateMs { get; }
        List<IContexMenuItem> ContextMenu { get; }
        void OnChildDelete(ITreeNode node);
        ITreeNode Copy();
        void LinklToParent(ITreeNode Parent);
        Type ParentType { get; }

        void OnChildrenChanded();
    }

    public interface IContexMenuItem
    {
        string Text { get; }
        ICommand Todo { get; }
    }

    public interface IConditionSource 
    {
        string SourceName { get; }
        string Value { get; }
        Type ValueType { get; }
    }
    public interface IReactionService:IService
    {
        void Check();
        void Check(params IViewModel[] parametersViewModel);
    }

    public interface IGlobalParams:IService
    {
        int? CurrentModeId { get; set; }
        int LogLevel { get; set; }
        bool LogDBAddRemove { get; set; }
    }
    public class GlobalParams : IGlobalParams
    {
        public int? CurrentModeId { get; set; }
        public void SetContainer(IServiceContainer container)
        {
            if(!File.Exists("options"))
                return;
            var opts = File.ReadAllText("options").Split(new []{'\r','\n'},StringSplitOptions.RemoveEmptyEntries);
            foreach (var opt in opts)
            {
                if (opt.ToLower().Contains("loglevel"))
                    LogLevel = int.Parse(opt.Split(new[] {"="}, StringSplitOptions.RemoveEmptyEntries)[1]);
                if (opt.ToLower().Contains("logdbaddremove"))
                    LogDBAddRemove = bool.Parse(opt.Split(new[] { "=" }, StringSplitOptions.RemoveEmptyEntries)[1]);
            }
        }

        public int LogLevel { get; set; }
        public bool LogDBAddRemove { get; set; }
    }

    
    public enum WebCommandType
    {
        GetParams,
        GetModes,
        SetParam,
        SetMode,
        GetModesJson,
        GetParamsJson
    }
    public interface ISettings:IService
    {
        int LogLevel { get; set; }
    }
    public interface ICopyService:IService
    {
        void SetCopyObject(ITreeNode copyObject);
        bool AllowPasteOn(ITreeNode parent);
        void PasteTo(ITreeNode parent);
        event Action CopyObjChanged;
    }
    public interface IInitable
    {
        void Init();
    }
}
