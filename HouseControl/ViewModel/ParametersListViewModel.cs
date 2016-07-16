using System;
using System.Collections.Generic;
using System.Linq;
using Facade;
using ViewModelBase;

namespace ViewModel
{
    public class ParametersListViewModel : ViewModelBase.ViewModelBase
    {
        public ParametersListViewModel(IServiceContainer container) : base(container)
        {
        }

        public override int ID { get; set; }
        private readonly Func<ParametårViewModel,bool> _filterCiteria = vm => true;
        public IEnumerable<ParametårViewModel> Parameters
        {
            get { return Use<IPool>().GetViewModels<ParametårViewModel>().Where(a=>_filterCiteria(a)); }
        }

        public void CreateParams()
        {
            var newParam = Use<IPool>().CreateDBObject<ParametårViewModel>();
            newParam.Name = "Ïàðàìåòð" ;
            newParam.Value = "";
            newParam.ParamType = Use<IPool>().GetViewModels<ParameterTypeViewModel>().First();
            Use<IPool>().SaveDB();
            OnPropertyChanged(() => Parameters);
        }
    }
}