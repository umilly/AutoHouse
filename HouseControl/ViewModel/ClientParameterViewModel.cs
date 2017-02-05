using Facade;
using Model;
using ViewModelBase;

namespace ViewModel
{
    public class ClientParameterViewModel:ViewModelBase.ViewModelBase
    {
        public ClientParameterViewModel(IServiceContainer container) : base(container)
        {
        }

        public ParameterProxy Param { get; set; }
        public override int ID
        {
            get { return Param.ID; }
            set {  }
        }

        public string Value => Param.Value;
        public ParameterTypeValue Type => Param.ParamType;
        public string Name => Param.Name;
        public int NextParam => Param.NextParam;

        public void Delete()
        {
            Use<IPool>().RemoveVM(typeof(ClientParameterViewModel), ID);
        }
    }
}