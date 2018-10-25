using Facade;
using Model;
using ViewModelBase;

namespace ViewModel
{
    public class ConditionTypeViewModel : EntityObjectVm<ConditionType>
    {
        public ConditionTypeViewModel(IServiceContainer container,  ConditionType model) : base(container, model)
        {
        }
        public string Name => Model.Name;

        public override bool Validate()
        {
            return true;
        }

        public ConditionTypeValue TypeValue => (ConditionTypeValue)Model.ID;

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