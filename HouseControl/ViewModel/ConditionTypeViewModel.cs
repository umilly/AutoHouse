using Facade;
using Model;
using ViewModelBase;

namespace ViewModel
{
    public class ConditionTypeViewModel : EntytyObjectVM<ConditionType>
    {
        public ConditionTypeViewModel(IServiceContainer container, Models dataBase, ConditionType model) : base(container, dataBase, model)
        {
        }
        public string Name => Model.Name;

        public override bool Validate()
        {
            return true;
        }

        public void LinkCondtioin(Condition model)
        {
            model.ConditionType = Model;
        }

        public override string ToString()
        {
            return Model.Name;
        }
    }
}