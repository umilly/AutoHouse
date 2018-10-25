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
            Filter.OnFilterChanged += () => { OnPropertyChanged(() => Parameters); };
        }

        private ParameterFilterVM _filter;
        public ParameterFilterVM Filter => _filter?? (_filter=Use<IPool>().GetOrCreateVM<ParameterFilterVM>(-1));
        public override int ID { get; set; }
        
        public List<IPublicParam> Parameters
        {
            get
            {
                var f = Filter;
                return
                    new[] {Filter}.Union(
                    Use<IPool>().GetViewModels<ParameterViewModel>().Where(a => f.IsMatch(a))
                    .Cast<IPublicParam>()).ToList();
            }
        }
        public void OnParamsChanged()
        {
            OnPropertyChanged(() => AllParams);
        }
        public IEnumerable<IConditionSource> AllParams
        {
            get
            {
                return
                    Use<IPool>()
                        .GetViewModels<ParameterViewModel>()
                        .Cast<IConditionSource>()
                        //.Except(new[] { this })
                        .Union(new[] { EmptyValue.Instance });
            }
        }

        public IEnumerable<ParameterTypeViewModel> ParamTypes => Use<IPool>().GetViewModels<ParameterTypeViewModel>();
        public IEnumerable<ParameterCategoryVm> Categories => Use<IPool>().GetViewModels<ParameterCategoryVm>();
        public IEnumerable<IConditionSource> Sensors => Use<IPool>().GetViewModels<ISensorVM>().Union(new[] { (IConditionSource)EmptyValue.Instance });


        public void CreateParams()
        {
            var newParam = Use<IPool>().CreateDBObject<ParameterViewModel>();
            newParam.Name = "Параметр" ;
            newParam.Value = "";
            newParam.ParamType = Use<IPool>().GetViewModels<ParameterTypeViewModel>().First();
            //Use<IPool>().SaveDB(TODO);
            OnPropertyChanged(() => Parameters);
        }

        public void ClearFilter()
        {
            Filter.Clear();
        }
    }
}