using Facade;
using Model;
using ViewModelBase;

public class ParametårViewModel : EntytyObjectVM<Parametår>
{
    public ParametårViewModel(IServiceContainer container, Models dataBase, Parametår model) : base(container, dataBase, model)
    {
    }
    public CommandViewModel Command
    {
        get { return Use<IPool>().GetDBVM<CommandViewModel>(Model.Command.ID); }
    }
    public override bool Validate()
    {
        return !string.IsNullOrEmpty(Model.Name) && Command != null;
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