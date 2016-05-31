using Facade;
using Model;
using ViewModelBase;

public class ReactionViewModel : EntytyObjectVM<Reaction>
{
    public ReactionViewModel(IServiceContainer container, Models dataBase, Reaction model) : base(container, dataBase, model)
    {
    }

    public ScenarioViewModel Scenario
    {
        get { return Use<IPool>().GetDBVM<ScenarioViewModel>(Model.ScenarioId); }
    }

    public override bool Validate()
    {
        return !string.IsNullOrEmpty(Model.Name);
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