using System.Collections.Generic;
using System.Linq;
using Facade;
using Model;
using ViewModel;
using ViewModelBase;

public class ScenarioViewModel : EntytyObjectVM<Scenario>,IDeviceTreeNode
{
    public ScenarioViewModel(IServiceContainer container, Models dataBase, Scenario model) : base(container, dataBase, model)
    {
        Children = Enumerable.Empty<IDeviceTreeNode>();
    }

    public IDeviceTreeNode Parent
    {
        get { return Use<IPool>().GetDBVM<ModeViewModel>(Model.Mode.ID); }
        set { Model.Mode.Id = (value as ModeViewModel).ID; }
    }

    public IEnumerable<IDeviceTreeNode> Children { get; }

    public string Value
    {
        get { return string.Empty; }
    }

    public bool IsConnected
    {
        get { return true; }
    }

    public IEnumerable<IContexMenuItem> ContextMenu { get; }

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

    public void LinkTo(Mode mode)
    {
        Model.Mode = mode;
        mode.Scenarios.Add(Model);
    }
}