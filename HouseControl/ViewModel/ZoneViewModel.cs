using Facade;
using Model;
using ViewModelBase;

namespace ViewModel
{
    public class ZoneViewModel : EntytyObjectVM<Zone>
    {
        
        public ZoneViewModel(IServiceContainer container, Models dataBase, Zone model) : base(container, dataBase, model)
        {
        }

        public string Name
        {
            get { return Model.Name; }
            set { Model.Name = value; }
        }

        public override bool Validate()
        {
            return !string.IsNullOrEmpty(Model.Name);
        }

        public bool IsLinkedWithScenario(Scenario model)
        {
            return model.Zones.Contains(Model);
        }

        public void LinkWithScenario(Scenario model, bool value)
        {
            if (value)
            {
                Model.Scenarios.Add(model);
                model.Zones.Add(Model);
            }
            else
            {
                Model.Scenarios.Remove(model);
                model.Zones.Remove(Model);
            }
        }
    }
}