using System.Collections.Generic;
using Facade;
using Model;
using ViewModelBase;

public class ConditionViewModel : LinkedObjectVM<Condition>
{
    public ConditionViewModel(IServiceContainer container, Models dataBase, Condition model) : base(container, dataBase, model)
    {
    }
    public ReactionViewModel Reaction => Model.ReactionId.HasValue? Use<IPool>().GetDBVM<ReactionViewModel>(Model.ReactionId.Value):null;

    public override ITreeNode Parent => Reaction;

    public override IEnumerable<ITreeNode> Children { get; }
    public override string Value { get; set; }
    public override bool IsConnected { get; set; }

    public override bool Validate()
    {
        return !string.IsNullOrEmpty(Model.Name)&&Reaction!=null;
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
}