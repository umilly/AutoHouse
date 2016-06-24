using System.Collections.Generic;
using System.Linq;
using Facade;
using Model;
using ViewModel;
using ViewModelBase;

public class ModeViewModel : EntytyObjectVM<Mode>,IDeviceTreeNode
{
    public ModeViewModel(IServiceContainer container, Models dataBase, Mode model) : base(container, dataBase, model)
    {
    }

    public IDeviceTreeNode Parent { get; }

    public IEnumerable<IDeviceTreeNode> Children
    {
        get { return Scenarios; } 
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

    public string Value
    {
        get { return string.Empty; }
    }

    public bool IsConnected { get { return true; } }

    public IEnumerable<IContexMenuItem> ContextMenu
    {
        get
        {
            yield return new CustomContextMenuItem("Добавить сценарий",new CommandHandler(AddScenario));
            yield return new CustomContextMenuItem("Удалить", new CommandHandler(DeleteMode));
        }
    }

    private void DeleteMode(bool obj)
    {
        Delete();
    }

    protected override void OnDelete()
    {
        Children.Cast<IEntytyObjectVM>().ForEach(a=>a.Delete());
        base.OnDelete();
    }

    private void AddScenario(bool obj)
    {
        var newScenario= Use<IPool>().CreateDBObject<ScenarioViewModel>();
        newScenario.Name = "Сценарий";
        newScenario.LinkTo(Model);
        OnPropertyChanged(()=>Children);
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