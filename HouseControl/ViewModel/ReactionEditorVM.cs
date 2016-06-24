using Facade;

namespace ViewModel
{
    public class ReactionEditorVM : ViewModelBase.ViewModelBase
    {
        public ReactionEditorVM(IServiceContainer container) : base(container)
        {
        }

        public override int ID
        {
            get { return 1; }
            set { }
        }
    }
}