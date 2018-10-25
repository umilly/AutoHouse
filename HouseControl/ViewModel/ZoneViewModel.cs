using Facade;
using Model;
using ViewModelBase;

namespace ViewModel
{
    public class ZoneViewModel : EntityObjectVm<Zone>
    {
        
        public ZoneViewModel(IServiceContainer container,Zone model) : base(container, model)
        {
        }

        public string Name
        {
            get { return Model.Name; }
            set { Model.Name = value; }
        }

        public string Key
        {
            get { return Model.Description; }
            set
            {
                Model.Description = value; 
                OnPropertyChanged();
            }
        }

        public bool IsGlobal
        {
            get { return Model.ID == 1; } 
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

        public void LinkSensor(Sensor model)
        {
            model.Zone = Model;
        }

        public void UnlinkScenario(Scenario model)
        {
            Model.Scenarios.Remove(model);
        }
    }
}