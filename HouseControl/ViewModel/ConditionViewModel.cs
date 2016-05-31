using Facade;
using Model;
using ViewModelBase;

public class ConditionViewModel : EntytyObjectVM<Condition>
{
    public ConditionViewModel(IServiceContainer container, Models dataBase, Condition model) : base(container, dataBase, model)
    {
    }
    public ReactionViewModel Reaction
    {
        get { return Model.ReactionId.HasValue? Use<IPool>().GetDBVM<ReactionViewModel>(Model.ReactionId.Value):null; }
    }
    public override bool Validate()
    {
        return !string.IsNullOrEmpty(Model.Name)&&Reaction!=null;
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