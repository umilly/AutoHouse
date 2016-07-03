using System.Collections.Generic;
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

        public override ITreeNode Parent => Reaction;

        public override IEnumerable<ITreeNode> Children { get; }
        public override string Value { get; set; }
        public override bool IsConnected { get; set; }

        public override bool Validate()
        {
            return !string.IsNullOrEmpty(Model.Name) && Reaction != null;
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