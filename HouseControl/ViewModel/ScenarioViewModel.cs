using Facade;
using Model;
using ViewModelBase;

public class ScenarioViewModel : EntytyObjectVM<Scenario>
{
    public ScenarioViewModel(IServiceContainer container, Models dataBase, Scenario model) : base(container, dataBase, model)
    {
    }

    public override bool Validate()
    {
        return !string.IsNullOrEmpty(Model.Name) && !string.IsNullOrEmpty(Model.Description);
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
    public string Description
    {
        get { return Model.Description; }
        set
        {
            Model.Description = value;
            OnPropertyChanged();
        }
    }
}