using System.Collections.Generic;
using Facade;
using Model;
using ViewModel;
using ViewModelBase;

public class ReactionViewModel : LinkedObjectVM<Reaction>, ITreeNode
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
    public override string Name
    {
        get { return Model.Name; }
        set
        {
            Model.Name = value;
            OnPropertyChanged();
        }
    }

    public override ITreeNode Parent => Scenario;
    public override IEnumerable<ITreeNode> Children { get; }
    public override string Value
    {
        get { return string.Empty; }
        set {  }
    }

    public override bool IsConnected
    {
        get { return true; }
        set { }
    }

    public void Link(Scenario model)
    {
        Model.Scenario = model;
    }
}