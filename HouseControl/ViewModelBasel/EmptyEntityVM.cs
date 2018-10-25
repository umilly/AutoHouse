using Facade;
using Model;

namespace ViewModelBase
{
    public abstract class EmptyEntityVM : EntityObjectVm<EmptyModel>
    {
        protected EmptyEntityVM(IServiceContainer container) : base(container,EmptyModel.Instance)
        {
        }

        public override bool Validate()
        {
            return true;
        }
    }
}