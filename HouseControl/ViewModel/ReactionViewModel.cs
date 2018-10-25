using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Facade;
using Model;
using ViewModel;
using ViewModelBase;

namespace ViewModel
{
    public class ReactionViewModel : LinkedObjectVm<Reaction>, ITreeNode, IConditionParent
    {
        public ReactionViewModel(IServiceContainer container, Reaction model)
            : base(container,  model)
        {
            _contextMenu.Add(new CustomContextMenuItem("Добавить условие",new CommandHandler(AddCondition)));
            _contextMenu.Add(new CustomContextMenuItem("Добавить команду", new CommandHandler(AddCommand)));
            _contextMenu.Add(new CustomContextMenuItem("Добавить команду управления параметром", new CommandHandler(AddCommandParam)));
        }

        public override Type ParentType { get { return typeof(ScenarioViewModel); } }

        private void AddCommandParam(bool obj)
        {
            var command = Use<IPool>().CreateDBObject<ParameterSetCommandVm>();
            command.LinkTo(Model);
            command.Name = "Команда параметра";
            OnPropertyChanged(() => Children);
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

        public void LinkCondition(Condition model)
        {
            model.Reaction = Model;
            OnPropertyChanged(() => Children);
        }
        public ScenarioViewModel Scenario
        {
            get { return Use<IPool>().GetOrCreateDBVM<ScenarioViewModel>(Model.Scenario); }
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

        public override void LinklToParent(ITreeNode newParent)
        {
            if (!(newParent is ScenarioViewModel))
                throw new InvalidEnumArgumentException("reaction's parent must be scenario");
            (newParent as ScenarioViewModel).LinkChildReaction(Model);
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
                        .Union(Use<IPool>().GetViewModels<CommandViewModel>().Where(a => a.Parent == this))
                        .Union(Use<IPool>().GetViewModels<ParameterSetCommandVm>().Where(a => a.Parent == this));
            } 
        }

        public override string Value
        {
            get { return string.Empty; }
            set { }
        }

        public override bool? IsConnected
        {
            get { return null; }
            set { }
        }

        public void Link(Scenario model)
        {
            Model.Scenario = model;
        }

        public void Check()
        {
            var conditions = Use<IPool>()
                .GetViewModels<ConditionViewModel>()
                .Where(a => a.Parent == this);

            if (conditions.All(a => a.CheckComplete()))
            {
                SendCommands();
            }
        }

        private void SendCommands()
        {
            Use<IPool>()
                .GetViewModels<CommandViewModel>()
                .Where(a => a.Parent == this)
                .ForEach(a => a.Execute());
            Use<IPool>()
                .GetViewModels<ParameterSetCommandVm>()
                .Where(a => a.Parent == this)
                .ForEach(a => a.Execute());

        }

        public void LinkChildCommand(Command model)
        {
            model.Reaction = Model;
            OnPropertyChanged(() => Children);
        }

        public void LinkChildParamCommand(ParametrSetCommand model)
        {
            model.Reaction = Model;
            OnPropertyChanged(() => Children);
        }
    }
}