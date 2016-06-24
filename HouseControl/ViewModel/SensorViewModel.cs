using System.Collections.Generic;
using System.Linq;
using Facade;
using Model;
using ViewModel;
using ViewModelBase;

public class SensorViewModel : EntytyObjectVM<Sensor>, IDeviceTreeNode
{
    public SensorViewModel(IServiceContainer container, Models dataBase, Sensor model)
        : base(container, dataBase, model)
    {
        Children = Enumerable.Empty<IDeviceTreeNode>();
    }

    public override bool Validate()
    {
        return Model.Controller != null
               && Model.ContollerSlot > 0
               && Model.SensorType != null
               && Model.Name != null
            ;
    }

    public IDeviceTreeNode Parent => _parent;

    private ControllerVM _parent => Use<IPool>().GetDBVM<ControllerVM>(Model.Controller.ID);

    public IEnumerable<IDeviceTreeNode> Children { get; }

    public string Name
    {
        get { return Model.Name; }
    }

    public string Value => (_parent!=null&&_parent.Values.ContainsKey(Model.ContollerSlot)) ? _parent.Values[Model.ContollerSlot] : "-";
    public bool IsConnected => true;
    public IEnumerable<IContexMenuItem> ContextMenu { get; }

    public void Init(SensorType st, int slotNum, Controller controller, string name)
    {
        Model.SensorType = st;
        Model.ContollerSlot = slotNum;
        Model.Controller = controller;
        Model.Name = name;
    }
    public void UpdateValue()
    {
        OnPropertyChanged(()=>Value);
    }
}