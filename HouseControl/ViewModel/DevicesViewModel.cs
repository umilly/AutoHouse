using System;
using System.Collections.Generic;
using Facade;
using Model;
using ViewModel;
using ViewModelBase;

public class DevicesViewModel : ViewModelBase.LinkedObjectVm<EmptyModel>, ITreeNode
{
    //readonly List<IContexMenuItem> _contextMenu = new List<IContexMenuItem>();

    public DevicesViewModel(IServiceContainer container,EmptyModel model) : base(container, model)
    {
        _contextMenu.Add(new CustomContextMenuItem("Добавить контроллер", new CommandHandler(AddController)));
        _contextMenu.Add(new CustomContextMenuItem("Добавить контроллер modbus", new CommandHandler(AddControllerMB)));
        
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

    public override ITreeNode Parent => null;

    public override IEnumerable<ITreeNode> Children => Use<IPool>().GetViewModels<ControllerVM>();

    public override string Name
    {
        get => "Устройства";
        set {  }
    }

    public override string Value
    {
        get => string.Empty;
        set {  }
    }

    public override int LastUpdateMs { get; } = 0;

    //public override List<IContexMenuItem> ContextMenu => _contextMenu;
    //public List<IContexMenuItem> ContextMenu { get; }

    public void OnChildDelete(ITreeNode scenarioViewModel)
    {
        OnPropertyChanged(() => Children);
    }

    public ITreeNode Copy()
    {
        throw new System.NotImplementedException();
    }

    public override void LinklToParent(ITreeNode Parent)
    {
        
    }

    public override Type ParentType { get { return null; } }

    private void AddController(bool obj)
    {
        var newMode = Use<IPool>().CreateDBObject<ControllerVM>();
        newMode.Name = "Контроллер";
        OnPropertyChanged(() => Children);
    }
    private void AddControllerMB(bool obj)
    {
        var newMode = Use<IPool>().CreateDBObject<ModbusControllerViewModel>();
        newMode.Name = "Контроллер ModBus";
        OnPropertyChanged(() => Children);
    }
}