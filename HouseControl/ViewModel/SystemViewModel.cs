using System;
using System.Collections.Generic;
using System.Windows.Input;
using Facade;
using ViewModel;
using ViewModelBase;

public class SystemViewModel : ViewModelBase.ViewModelBase, IDeviceTreeNode
{
    public SystemViewModel(IServiceContainer container) : base(container)
    {
    }

    public override int ID
    {
        get { return -1; }
        set { }
    }

    public IDeviceTreeNode Parent => null;

    public IEnumerable<IDeviceTreeNode> Children => Use<IPool>().GetViewModels<ModeViewModel>();

    public string Name => "Вся система";

    public string Value => string.Empty;

    public bool IsConnected => true;

    public IEnumerable<IContexMenuItem> ContextMenu
    {
        get { yield return new CustomContextMenuItem("Добавить режим", new CommandHandler(AddMode)); }
    }

    private void AddMode(bool obj)
    {
        var newMode=Use<IPool>().CreateDBObject<ModeViewModel>();
        newMode.Name = "Режим";
        OnPropertyChanged(()=>Children);
    }
}