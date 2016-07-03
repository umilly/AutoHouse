using System.Collections.Generic;
using Facade;
using ViewModelBase;

namespace ViewModel
{
    public class DevicesEditorVM : ViewModelBase.ViewModelBase
    {
        public DevicesEditorVM(IServiceContainer container) : base(container)
        {
        }

        public override int ID
        {
            get { return 1; }
            set { }
        }

        public IEnumerable<ITreeNode> Reactions
        {
            get { yield return Use<IPool>().GetOrCreateVM<DevicesViewModel>(-1); }
        }
    }
}