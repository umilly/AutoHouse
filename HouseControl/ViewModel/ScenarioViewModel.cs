using System.Collections.Generic;
using System.Linq;
using Facade;
using Model;
using ViewModel;
using ViewModelBase;

public class ScenarioViewModel : LinkedObjectVM<Scenario>,ITreeNode
{
    public ScenarioViewModel(IServiceContainer container, Models dataBase, Scenario model) : base(container, dataBase, model)
    {
        _contextMenu.Add(new CustomContextMenuItem("Добавить реакцию", new CommandHandler(AddReaction)));


    }

    public override ITreeNode Parent => Model.Mode==null?null: Use<IPool>().GetDBVM<ModeViewModel>(Model.Mode.ID);

    public override IEnumerable<ITreeNode> Children => Use<IPool>().GetViewModels<ReactionViewModel>().Where(a=>a.Scenario==this);

    public override string Value
    {
        get { return string.Empty; }
        set {  }
    }

    public override bool IsConnected
    {
        get { return true; }
        set {  }
    }

    private void AddReaction(bool obj)
    {
        var newReaction= Use<IPool>().CreateDBObject<ReactionViewModel>();
        newReaction.Name = "Реакция";
        newReaction.Link(Model);
        OnPropertyChanged(()=>Children);
    }

    public override bool Validate()
    {
        return !string.IsNullOrEmpty(Model.Name) && !string.IsNullOrEmpty(Model.Description);
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