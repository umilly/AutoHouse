using System.Collections.Generic;
using Facade;
using Model;
using ViewModelBase;

namespace ViewModel
{
    public class Paramet�rViewModel : EntytyObjectVM<Paramet�r>
    {
        public Paramet�rViewModel(IServiceContainer container, Models dataBase, Paramet�r model)
            : base(container, dataBase, model)
        {
            
        }

        public override bool Validate()
        {
            return !string.IsNullOrEmpty(Model.Name)&&Model.ParameterType!=null;
        }

        public string Value
        {
            get { return Model.Value; }
            set
            {
                Model.Value = value;
                OnPropertyChanged();
            }
        }

        public IEnumerable<ParameterTypeViewModel> ParamTypes => Use<IPool>().GetViewModels<ParameterTypeViewModel>();

        public ParameterTypeViewModel ParamType
        {
            get { return Use<IPool>().GetDBVM<ParameterTypeViewModel>(Model.ParameterType); }
            set
            {
                value.LinkParam(Model);
            }
        }
        public string Name
        {
            get { return Model.Name; }
            set
            {
                Model.Name = value;
                OnPropertyChanged();
            }
        }
    }
}