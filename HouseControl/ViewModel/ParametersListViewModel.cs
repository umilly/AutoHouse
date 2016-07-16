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
        private readonly Func<Paramet�rViewModel,bool> _filterCiteria = vm => true;
        public IEnumerable<Paramet�rViewModel> Parameters
        {
            get { return Use<IPool>().GetViewModels<Paramet�rViewModel>().Where(a=>_filterCiteria(a)); }
        }

        public void CreateParams()
        {
            var newParam = Use<IPool>().CreateDBObject<Paramet�rViewModel>();
            newParam.Name = "��������" ;
            newParam.Value = "";
            newParam.ParamType = Use<IPool>().GetViewModels<ParameterTypeViewModel>().First();
            Use<IPool>().SaveDB();
            OnPropertyChanged(() => Parameters);
        }
    }
}