using System.Collections.Generic;
using System.Linq;
using Facade;
using Model;
using ViewModelBase;

namespace ViewModel
{
    public class ClientParametersViewModel:ViewModelBase.ViewModelBase
    {
        public ClientParametersViewModel(IServiceContainer container) : base(container)
        {
        }
        public override int ID { get { return 1; } set {} }

        public List<ClientParameterViewModel> Parameters => Use<IPool>().GetViewModels<ClientParameterViewModel>().ToList();

        public string Url
            =>
                $"http://{Use<IPool>().GetViewModels<ClientOptionsViewModel>().Single().ServerIP}:{Use<IPool>().GetViewModels<ClientOptionsViewModel>().Single().ServerPort}/{WebCommandType.GetParamsJson}"
            ;

        public void AskParams()
        {
            var paramSS=Use<INetworkService>().SyncRequest(Url);
            var des = Use<INetworkService>().Deserialize<Parameters>(paramSS);
            Use<IPool>().GetViewModels<ClientParameterViewModel>().ForEach(a=>a.Delete());
            foreach (var parameterProxy in des.ParamList)
            {
                var para=Use<IPool>().CreateVM<ClientParameterViewModel>();
                para.Param = parameterProxy;
            }
            OnPropertyChanged(()=> Parameters);
        }
    }
}