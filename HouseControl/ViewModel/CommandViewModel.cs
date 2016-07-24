using System.Collections.Generic;
using System.Linq;
using Facade;
using Model;
using ViewModel;
using ViewModelBase;

namespace ViewModel
{
    public class CommandViewModel : LinkedObjectVM<Command>
    {
        public CommandViewModel(IServiceContainer container, Models dataBase, Command model)
            : base(container, dataBase, model)
        {
            
        }

        public ReactionViewModel Reaction => Use<IPool>().GetDBVM<ReactionViewModel>(Model.Reaction);

        public override ITreeNode Parent => Reaction;
        public override IEnumerable<ITreeNode> Children { get; }
        public override string Value { get; set; }
        public override bool IsConnected { get; set; }

        public override bool Validate()
        {
            return !string.IsNullOrEmpty(Model.Name) && Reaction != null;
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

        public IEnumerable<CustomDeviceViewModel> Devices
        {
            get {return Use<IPool>().GetViewModels<CustomDeviceViewModel>(); }
        }

        public CustomDeviceViewModel Device
        {
            get { return Use<IPool>().GetDBVM<CustomDeviceViewModel>(Model.CustomDevice); }
            set
            {
                Parameters.ForEach(a=>a.Delete());
                value.LinkToCommand(Model);
                CreateCommandLinks();
                OnPropertyChanged();
            }
        }

        private void CreateCommandLinks()
        {
            foreach (var  devParam in Device.ParameterTypes.OrderBy(a=>a.Order))
            {
                var link=Use<IPool>().CreateDBObject<CommandParamLinkVm>();
                link.DeviceParamType = devParam;
                link.Command = this;
            }
            OnPropertyChanged(()=>Parameters);
        }

        public IEnumerable<CommandParamLinkVm> Parameters
        {
            get { return Model.ComandParameterLinks.Select(a => Use<IPool>().GetDBVM<CommandParamLinkVm>(a)); }
        }

        public void LinkTo(Reaction model)
        {
            Model.Reaction = model;
        }

        public void LinkCommandParam(ComandParameterLink model)
        {
            model.Command = Model;
            Model.ComandParameterLinks.Add(model);
        }
    }
}