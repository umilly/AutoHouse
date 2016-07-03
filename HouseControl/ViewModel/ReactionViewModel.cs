using System;
using System.Collections.Generic;
using System.Linq;
using Facade;
using Model;
using ViewModel;
using ViewModelBase;

namespace ViewModel
{
    public class ReactionViewModel : LinkedObjectVM<Reaction>, ITreeNode
    {
        public ReactionViewModel(IServiceContainer container, Models dataBase, Reaction model)
            : base(container, dataBase, model)
        {
            _contextMenu.Add(new CustomContextMenuItem("Добавить условие",new CommandHandler(AddCondition)));
            _contextMenu.Add(new CustomContextMenuItem("Добавить команду", new CommandHandler(AddCommand)));
        }

        private void AddCommand(bool obj)
        {
            var command = Use<IPool>().CreateDBObject<CommandViewModel>();
            command.LinkTo(Model);
            command.Name = "Команда";
            OnPropertyChanged(() => Children);
        }

        private void AddCondition(bool b)
        {
            var condition = Use<IPool>().CreateDBObject<ConditionViewModel>();
            condition.LinkTo(Model);
            condition.Name = "Условие";
            OnPropertyChanged(() => Children);
        }

        public ScenarioViewModel Scenario
        {
            get { return Use<IPool>().GetDBVM<ScenarioViewModel>(Model.Scenario); }
        }

        public override bool Validate()
        {
            return !string.IsNullOrEmpty(Model.Name);
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
        public override ITreeNode Parent => Scenario;

        public override IEnumerable<ITreeNode> Children
        {
            get
            {
                return
                    Use<IPool>()
                        .GetViewModels<ConditionViewModel>().Where(a=>a.Parent==this)
                        .Cast<ITreeNode>()
                        .Union(Use<IPool>().GetViewModels<CommandViewModel>().Where(a => a.Parent == this));
            } 
        }

        public override string Value
        {
            get { return string.Empty; }
            set { }
        }

        public override bool IsConnected
        {
            get { return true; }
            set { }
        }

        public void Link(Scenario model)
        {
            Model.Scenario = model;
        }
    }
}