using System.Collections.Generic;
using Facade;
using Model;
using ViewModelBase;

namespace ViewModel
{
    public class Paramet�rViewModel : LinkedObjectVM<Paramet�r>
    {
        public Paramet�rViewModel(IServiceContainer container, Models dataBase, Paramet�r model)
            : base(container, dataBase, model)
        {
        }

        public override ITreeNode Parent { get; }
        public override IEnumerable<ITreeNode> Children { get; }
        public override string Value { get; set; }
        public override bool IsConnected { get; set; }

        public override bool Validate()
        {
            return !string.IsNullOrEmpty(Model.Name);
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
    }
}