using System.Collections.Generic;
using System.Linq;
using Facade;
using Model;
using ViewModel;
using ViewModelBase;

public class SensorViewModel : LinkedObjectVM<Sensor>, ITreeNode
{
    public SensorViewModel(IServiceContainer container, Models dataBase, Sensor model)
        : base(container, dataBase, model)
    {
        Children = Enumerable.Empty<ITreeNode>();
    }

    public override bool Validate()
    {
        return Model.Controller != null
               && Model.ContollerSlot > 0
               && Model.SensorType != null
               && Model.Name != null
            ;
    }

    public override ITreeNode Parent => Use<IPool>().GetDBVM<ControllerVM>(Model.Controller.ID);

    
    public override IEnumerable<ITreeNode> Children { get; }

    public override string Name
    {
        get { return Model.Name; }
        set { Model.Name = value; }
    }

    public override string Value
    {
        get
        {
            var p = Parent as ControllerVM;
            return (p != null && p.Values.ContainsKey(Model.ContollerSlot))
                ? p.Values[Model.ContollerSlot]
                : "-";
        }
        set { }
    }

    public override bool IsConnected
    {
        get { return Parent != null && (Parent as ControllerVM).Values.ContainsKey(Model.ContollerSlot); }
        set {  }
    }

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