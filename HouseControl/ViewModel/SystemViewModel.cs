using System;
using System.Collections.Generic;
using System.Windows.Input;
using Facade;
using ViewModel;
using ViewModelBase;

public class SystemViewModel : ViewModelBase.ViewModelBase, ITreeNode
{
    readonly List<IContexMenuItem> _contextMenu=new List<IContexMenuItem>();

    public SystemViewModel(IServiceContainer container) : base(container)
    {
        _contextMenu.Add(new CustomContextMenuItem("Добавить режим", new CommandHandler(AddMode)));
    }

    public override int ID
    {
        get { return -1; }
        set { }
    }

    public ITreeNode Parent => null;

    public IEnumerable<ITreeNode> Children => Use<IPool>().GetViewModels<ModeViewModel>();

    public string Name => "Вся система";

    public string Value => string.Empty;

    public bool IsConnected => true;

    public List<IContexMenuItem> ContextMenu => _contextMenu;

    public void OnChildDelete(ITreeNode scenarioViewModel)
    {
        OnPropertyChanged(() => Children);
    }

    private void AddMode(bool obj)
    {
        var newMode=Use<IPool>().CreateDBObject<ModeViewModel>();
        newMode.Name = "Режим";
        OnPropertyChanged(()=>Children);
    }
}