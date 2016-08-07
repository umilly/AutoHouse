using System;
using System.Collections.Generic;
using Facade;
using Model;
using ViewModelBase;

namespace ViewModel
{
    public class ParameterViewModel : EntytyObjectVM<Parameter>,IConditionSource
    {
        public ParameterViewModel(IServiceContainer container, Models dataBase, Parameter model)
            : base(container, dataBase, model)
        {
            if(!IsFake && Model.ID == Parameter.CurrentTimeId)
                Use<ITimerSerivce>().Subsctibe(this,UpdateTime,1000,true);
        }

        public Type ValueType => ParamType.Type;

        private void UpdateTime()
        {
            OnPropertyChanged(()=>Value);
        }

        public override bool Validate()
        {
            return !string.IsNullOrEmpty(Model.Name)&&Model.ParameterType!=null;
        }

        public string Value
        {
            get { return Model.ID == Parameter.CurrentTimeId ? DateTime.Now.ToLongTimeString() : Model.Value; }
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

        public bool IsEditable => Model.ID != Parameter.CurrentTimeId;

        public void LinkCondition(Condition model, bool firstParam)
        {
            if (firstParam)
            {
                model.Parameter1 = Model;
            }
            else
            {
                model.Parameter2 = Model;
            }
        }

        public string SourceName => $"Param: {Name}";

        public bool IsPublic
        {
            get { return Model.IsPublic; }
            set
            {
                Model.IsPublic = value;
                OnPropertyChanged();
            }
        }

        public void LinkDeviceParam(ComandParameterLink model)
        {
            model.Parameter = Model;
        }
    }
}