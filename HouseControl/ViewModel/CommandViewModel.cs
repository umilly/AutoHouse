using Facade;
using Model;
using ViewModelBase;

public class CommandViewModel : EntytyObjectVM<Command>
{
    public CommandViewModel(IServiceContainer container, Models dataBase, Command model) : base(container, dataBase, model)
    {
    }
    public ReactionViewModel Reaction
    {
        get { return Use<IPool>().GetDBVM<ReactionViewModel>(Model.ReactionId); }
    }
    public override bool Validate()
    {
        return !string.IsNullOrEmpty(Model.Name) && Reaction != null;
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