using System.Collections.Generic;
using System.Linq;
using Facade;
using Model;
using ViewModelBase;

public class ModeViewModel : EntytyObjectVM<Mode>
{
    public ModeViewModel(IServiceContainer container, Models dataBase, Mode model) : base(container, dataBase, model)
    {
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

    public IEnumerable<ScenarioViewModel> Scenarios
    {
        get { return Model.Scenarios.Select(a => Use<IPool>().GetDBVM<ScenarioViewModel>(a.ID)); }
    }

    public override bool Validate()
    {
        return !string.IsNullOrEmpty(Model.Name) && !string.IsNullOrEmpty(Model.Description);
    }
}