using System;
using System.Collections.Generic;
using System.Windows.Input;
using Facade;
using Model;
using ViewModel;
using ViewModelBase;

public class SystemViewModel : ViewModelBase.LinkedObjectVm<EmptyModel>, ITreeNode
{
    readonly List<IContexMenuItem> _contextMenu=new List<IContexMenuItem>();

    public SystemViewModel(IServiceContainer container,EmptyModel model) : base(container, model)
    {
        _contextMenu.Add(new CustomContextMenuItem("Добавить режим", new CommandHandler(AddMode)));
    }

    public override int ID
    {
        get { return -1; }
        set { }
    }

    public override bool Validate()
    {
        return true;
    }

    public override Type ParentType => null;

    public override ITreeNode Parent => null;

    public override  IEnumerable<ITreeNode>  Children => Use<IPool>().GetViewModels<ModeViewModel>();

    public override string Name
    {
        get => "Вся система";
        set {  }
    }

    public override string Value
    {
        get => string.Empty;
        set {  }
    }

    public override bool? IsConnected
    {
        get => null;
        set{}
    }

    public override int LastUpdateMs { get; }
    

    public override ITreeNode Copy()
    {
        throw new NotImplementedException();
    }

    public override void LinklToParent(ITreeNode Parent)
    {
        throw new NotImplementedException();
    }

    private void AddMode(bool obj)
    {
        var newMode=Use<IPool>().CreateDBObject<ModeViewModel>();
        newMode.Name = "Режим";
        OnPropertyChanged(()=>Children);
    }
}