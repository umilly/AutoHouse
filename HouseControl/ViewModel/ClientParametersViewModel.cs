using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Facade;
using Model;
using ViewModelBase;

namespace ViewModel
{
    public class ClientParametersViewModel:ViewModelBase.ViewModelBase
    {
        public ClientParametersViewModel(IServiceContainer container) : base(container)
        {
            _groupingService.Id = proxy => proxy.ZontId;
            _groupingService.Name= proxy => proxy.ZoneName;
        }
        public override int ID { get { return 1; } set {} }
        readonly GroupBy<ClientParameterViewModel>  _groupingService=new GroupBy<ClientParameterViewModel>();
        public List<ClientParameterViewModel> Parameters => Use<IPool>().GetViewModels<ClientParameterViewModel>().Where(a=>a.IsFirst).ToList();

        public string Url
            =>
                $"http://{Use<IPool>().GetViewModels<ClientOptionsViewModel>().Single().ServerIP}:{Use<IPool>().GetViewModels<ClientOptionsViewModel>().Single().ServerPort}/{WebCommandType.GetParamsJson}"
            ;
        private void AskParams()
        {
            Use<ITimerSerivce>().UnSubsctibe(this);
            Use<ITimerSerivce>().Subscribe(this,ParseParamsData,1000,true );
        }

        public List<Group<ClientParameterViewModel>> Groups => _groupingService.GetGroups(Parameters.ToList());
        public Page Page { get; set; }

        private void ParseParamsData()
        {
            var paramSS = Use<INetworkService>().SyncRequest(Url);
            if (string.IsNullOrEmpty(paramSS))
                return;
            var des = Use<INetworkService>().Deserialize<Parameters>(paramSS);
            var oldParams=Use<IPool>().GetViewModels<ClientParameterViewModel>().ToList();
            foreach (var parameterProxy in des.ParamList)
            {
                var para = Use<IPool>().GetOrCreateVM<ClientParameterViewModel>(parameterProxy.ID);
                para.Param = parameterProxy;
                if (oldParams.Contains(para))
                    oldParams.Remove(para);
            }
            oldParams.ForEach(a => a.Delete());
            OnPropertyChanged(() => Parameters);
            OnPropertyChanged(() => Groups);
            _onReceiveParams?.Invoke();
        }

        Action _onReceiveParams;
        public void AskParams(Action onReceiveParams)
        {
            _onReceiveParams = onReceiveParams;
            AskParams();
        }
    }
    public class Group<T>
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public List<T> Members { get; set; }

    }

    public class GroupBy<T>
    {
        private Expression<Func<T,int>> _id;
        private Func<T,int> _getIdDelegate;

        public Expression<Func<T,int>> Id
        {
            private get { return _id; }
            set
            {
                _id = value;
                _getIdDelegate = value?.Compile();
            }
        }
        private Expression<Func<T,string>> _name;
        private Func<T,string> _getNameDelegate;

        public Expression<Func<T,string>> Name
        {
            private get { return _name; }
            set
            {
                _name = value;
                _getNameDelegate = value?.Compile();
            }
        }

        public List<Group<T>> GetGroups(List<T> notGroupedList)
        {
            var grouped = notGroupedList.GroupBy(a => _getIdDelegate(a))
                .Select(g => new Group<T>()
                {
                    Id = g.Key,
                    Name = _getNameDelegate(g.First()),
                    Members = g.ToList()
                });
            var result = grouped.ToList();
            return result;
        }
    }
    public enum Page
    {
        Main,
        Climax,
        Light,
        Remote,
        Cameras,
        Access,
        Settings
    }
}