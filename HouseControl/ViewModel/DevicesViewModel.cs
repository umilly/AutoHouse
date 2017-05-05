using System.Collections.Generic;
using Facade;
using Model;
using ViewModel;
using ViewModelBase;

public class DevicesViewModel : ViewModelBase.ViewModelBase, ITreeNode
{
    readonly List<IContexMenuItem> _contextMenu = new List<IContexMenuItem>();

    public DevicesViewModel(IServiceContainer container) : base(container)
    {
        _contextMenu.Add(new CustomContextMenuItem("�������� ����������", new CommandHandler(AddController)));
        _contextMenu.Add(new CustomContextMenuItem("�������� ���������� modbus", new CommandHandler(AddControllerMB)));
    }

    public override int ID
    {
        get { return -1; }
        set { }
    }

    public ITreeNode Parent => null;

    public IEnumerable<ITreeNode> Children => Use<IPool>().GetViewModels<ControllerVM>();

    public string Name => "����������";

    public string Value => string.Empty;

    public bool? IsConnected => null;

    public List<IContexMenuItem> ContextMenu => _contextMenu;

    public void OnChildDelete(ITreeNode scenarioViewModel)
    {
        OnPropertyChanged(() => Children);
    }

    private void AddController(bool obj)
    {
        var newMode = Use<IPool>().CreateDBObject<ControllerVM>();
        newMode.Name = "����������";
        OnPropertyChanged(() => Children);
    }
    private void AddControllerMB(bool obj)
    {
        var newMode = Use<IPool>().CreateDBObject<ModbusControllerViewModel>();
        newMode.Name = "���������� ModBus";
        OnPropertyChanged(() => Children);
    }
}