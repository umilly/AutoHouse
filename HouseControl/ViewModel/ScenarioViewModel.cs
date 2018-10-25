using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Facade;
using Model;
using ViewModel;
using ViewModelBase;

namespace ViewModel
{
    public class ScenarioViewModel : LinkedObjectVm<Scenario>, ITreeNode
    {
        public ScenarioViewModel(IServiceContainer container, Models dataBase, Scenario model)
            : base(container, dataBase, model)
        {
            _contextMenu.Add(new CustomContextMenuItem("Добавить реакцию", new CommandHandler(AddReaction)));
        }

        public override void LinklToParent(ITreeNode newParent)
        {
            if (!(newParent is ModeViewModel))
                throw new InvalidEnumArgumentException("scenario's parent must be mode");
            (newParent as ModeViewModel).LinkChildScenario(Model);
        }
        public override Type ParentType { get { return typeof(ModeViewModel); } }

        public override ITreeNode Parent
            => Model.Mode == null ? null : Use<IPool>().GetOrCreateDBVM<ModeViewModel>(Model.Mode);

        public override IEnumerable<ITreeNode> Children
            => Use<IPool>().GetViewModels<ReactionViewModel>().Where(a => a.Scenario == this);

        public override string Value
        {
            get { return string.Empty; }
            set { }
        }

        public override bool? IsConnected
        {
            get { return null; }
            set { }
        }

        private void AddReaction(bool obj)
        {
            var newReaction = Use<IPool>().CreateDBObject<ReactionViewModel>();
            newReaction.Name = "Реакция";
            newReaction.Link(Model);
            OnPropertyChanged(() => Children);
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

        public IEnumerable<ZoneForScenario> Zones => Use<IPool>()
            .GetViewModels<ZoneViewModel>()
            .Select(a => new ZoneForScenario(this, a));

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

        public bool HaveZone(ZoneViewModel zone)
        {
            return zone.IsLinkedWithScenario(Model);
        }

        public void LinkZone(ZoneViewModel zone, bool value)
        {
            zone.LinkWithScenario(Model, value);
            ClearSensorsForNotValidZones();
        }

        private void ClearSensorsForNotValidZones()
        {
            var revalidateConditions = Use<IPool>()
                .GetViewModels<ConditionViewModel>()
                .Where(a => a.CurrentScenario == this);
            foreach (var conditionViewModel in revalidateConditions)
            {
                if (conditionViewModel.LeftParam is ISensorVM
                    && !HaveZone((conditionViewModel.LeftParam as ISensorVM).Zone))
                {
                    conditionViewModel.LeftParam = EmptyValue.Instance;
                }
            }
            var revalidateCommands = Use<IPool>()
                .GetViewModels<ParameterSetCommandVm>()
                .Where(a => a.Reaction.Scenario == this);
            foreach (var command in revalidateCommands)
            {
                if (command.Sensor != null
                    && !HaveZone((command.Sensor).Zone))
                {
                    command.Sensor = null;
                }
            }
        }

        public override void Delete()
        {
            foreach (var zoneForScenario in Zones)
            {
                zoneForScenario.Zone.UnlinkScenario(Model);
            }
            base.Delete();
        }

        public void LinkChildReaction(Reaction model)
        {
            model.Scenario = Model;
            OnPropertyChanged(()=>Children);
        }
    }
}

public class ZoneForScenario:INotifyPropertyChanged
{
    public ScenarioViewModel Scenario { get; set; }
    public ZoneViewModel Zone { get; set; }

    public ZoneForScenario(ScenarioViewModel scenario, ZoneViewModel zone)
    {
        Scenario = scenario;
        Zone = zone;
    }
    public string Name => Zone.Name;

    public bool IsLinked
    {
        get {return  Scenario.HaveZone(Zone); }
        set
        {
            Scenario.LinkZone(Zone, value);
            PropertyChanged?.Invoke(this,new PropertyChangedEventArgs("IsLinked"));
        }
    }

    public event PropertyChangedEventHandler PropertyChanged;
}