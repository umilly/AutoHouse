using System.Collections.Generic;
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
                value.LinkToCommand(Model);
                OnPropertyChanged();
            }
        }

        public IEnumerable<ParameterViewModel> Params
        {
            get { return Use<IPool>().GetViewModels<ParameterViewModel>(); }
        }

        public void LinkTo(Reaction model)
        {
            Model.Reaction = model;
        }
    }
}