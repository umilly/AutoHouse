using System.Collections.Generic;
using System.Linq;
using Facade;
using Model;
using ViewModel;
using ViewModelBase;

namespace ViewModel
{
    public class ModeViewModel : LinkedObjectVM<Mode>
    {
        private readonly List<IContexMenuItem> _contextMenu;

        public ModeViewModel(IServiceContainer container, Models dataBase, Mode model)
            : base(container, dataBase, model)
        {
            _contextMenu = base.ContextMenu;
            _contextMenu.Add(new CustomContextMenuItem("Добавить сценарий", new CommandHandler(AddScenario)));
            _contextMenu.Add(new CustomContextMenuItem("Копировать", new CommandHandler(CopyMode)));
        }

        private void CopyMode(bool obj)
        {
            Copy();
            (Parent as SystemViewModel).OnChildDelete(this);
        }

        public override void LinklToParent(ITreeNode Parent)
        {
            throw new System.NotImplementedException();
        }

        public override ITreeNode Parent => Use<IPool>().GetOrCreateVM<SystemViewModel>(-1);

        public override IEnumerable<ITreeNode> Children => Scenarios;

        public override string Name
        {
            get { return Model.Name; }
            set
            {
                Model.Name = value;
                OnPropertyChanged();
            }
        }

        public override string Value
        {
            get { return string.Empty; }
            set { }
        }

        public override bool? IsConnected
        {
            get
            {
                return IsSelected?(bool?)true:null;
            }
            set { }
        }

        private void AddScenario(bool obj)
        {
            var newScenario = Use<IPool>().CreateDBObject<ScenarioViewModel>();
            newScenario.Name = "Сценарий";
            newScenario.LinkTo(Model);
            OnPropertyChanged(() => Children);
        }

        public string Description
        {
            get { return Model.Description; }
            set
            {
                Model.Description = value;
                OnPropertyChanged();
            }
        }

        public IEnumerable<ScenarioViewModel> Scenarios
        {
            get { return Use<IPool>().GetViewModels<ScenarioViewModel>().Where(a=>a.Parent==this); }
        }

        public bool IsSelected
        {
            get { return Model.IsSelected; }
            set
            {
                Model.IsSelected = value; 
                OnPropertyChanged(()=>IsConnected);
            }
        }

        public ModeProxy GetProxy => ModeProxy.FromDBMode(Model);

        public override bool Validate()
        {
            return !string.IsNullOrEmpty(Model.Name) && !string.IsNullOrEmpty(Model.Description);
        }

        public void LinkChildScenario(Scenario model)
        {
            model.Mode = Model;
        }
    }
}