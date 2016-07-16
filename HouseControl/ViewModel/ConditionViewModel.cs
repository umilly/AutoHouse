using System.Collections.Generic;
using System.Linq;
using Facade;
using Model;
using ViewModelBase;

namespace ViewModel
{
    public class ConditionViewModel : LinkedObjectVM<Condition>
    {
        public ConditionViewModel(IServiceContainer container, Models dataBase, Condition model)
            : base(container, dataBase, model)
        {
            
        }

        public ReactionViewModel Reaction
            => Model.Reaction!=null ? Use<IPool>().GetDBVM<ReactionViewModel>(Model.Reaction) : null;

        public override ITreeNode Parent => Reaction ?? ParentCondition;
        public ITreeNode ParentCondition => Use<IPool>().GetDBVM<ConditionViewModel>(Model.ParentCondition);

        public override IEnumerable<ITreeNode> Children => Use<IPool>().GetViewModels<ConditionViewModel>().Where(a=>Parent==this);
        public override string Value { get; set; }
        public override bool IsConnected { get; set; }

        public override bool Validate()
        {
            return !string.IsNullOrEmpty(Model.Name) && Reaction != null;
        }

        public ParameterViewModel Parameter1 => Use<IPool>().GetDBVM<ParameterViewModel>(Model.Parameter1);
        public ParameterViewModel Parameter2 => Use<IPool>().GetDBVM<ParameterViewModel>(Model.Parameter2);
        public SensorViewModel Sensor => Use<IPool>().GetDBVM<SensorViewModel>(Model.Sensor);

        public IEnumerable<ConditionTypeViewModel> CondtionTypes
        {
            get
            {
                if (Children.OfType<ConditionViewModel>().Any())
                {
                    yield return Use<IPool>().GetDBVM<ConditionTypeViewModel>((int) ConditionTypeValue.And);
                    yield return Use<IPool>().GetDBVM<ConditionTypeViewModel>((int)ConditionTypeValue.Or);
                    yield break;
                }
                if (Parameter1!=null||Parameter2!=null||Sensor!=null)
                {

                    yield return Use<IPool>().GetDBVM<ConditionTypeViewModel>((int)ConditionTypeValue.Equal);
                    yield return Use<IPool>().GetDBVM<ConditionTypeViewModel>((int)ConditionTypeValue.NotEqual);
                    yield return Use<IPool>().GetDBVM<ConditionTypeViewModel>((int)ConditionTypeValue.More);
                    yield return Use<IPool>().GetDBVM<ConditionTypeViewModel>((int)ConditionTypeValue.Less);
                    yield break;
                }
                foreach (var ctype in Use<IPool>().GetViewModels<ConditionTypeViewModel>())
                {
                    yield return ctype;
                }

            }
        }

        public ConditionTypeViewModel CondtionType
        {
            get { return Use<IPool>().GetDBVM<ConditionTypeViewModel>(Model.ConditionType); }
            set
            {
                value.LinkCondtioin(Model);
                FillConextMenu();
            }
        }

        private void FillConextMenu()
        {
            _contextMenu.Clear();
            if (CondtionType.ID == (int) ConditionTypeValue.And ||
                CondtionType.ID == (int) ConditionTypeValue.Or)
            {
                _contextMenu.Add(new CustomContextMenuItem("Новое подусловие", new CommandHandler(b => AddChildCondition())));
            }
            _contextMenu.Add(new CustomContextMenuItem("Удалить",new CommandHandler(b => Delete())));
            OnPropertyChanged(()=>ContextMenu);
        }

        private void AddChildCondition()
        {
            var condition = Use<IPool>().CreateDBObject<ConditionViewModel>();
            condition.Model.ParentCondition = Model;
            condition.Name = "Условие";
            OnPropertyChanged(() => Children);
        }

        public override string Name
        {
            get { return Model.Name; }
            set
            {
                Model.Name = value;
                OnPropertyChanged();
            }
        }
        public void LinkTo(Reaction model)
        {
            Model.Reaction = model;
        }
    }
}