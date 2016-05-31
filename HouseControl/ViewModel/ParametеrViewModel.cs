using Facade;
using Model;
using ViewModelBase;

public class Paramet�rViewModel : EntytyObjectVM<Paramet�r>
{
    public Paramet�rViewModel(IServiceContainer container, Models dataBase, Paramet�r model) : base(container, dataBase, model)
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