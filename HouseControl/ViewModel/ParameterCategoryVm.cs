using Facade;
using Model;
using ViewModelBase;

public class ParameterCategoryVm : EntityObjectVm<ParameterCategory>
{
    public ParameterCategoryVm(IServiceContainer container, ParameterCategory model) : base(container, model)
    {
    }

    public override bool Validate()
    {
        return true;
    }

    public string Name
    {
        get { return Model.Name; }
        set
        {
            Model.Name = value;
            OnPropertyChanged();
        }
    }

    public void LinkParam(Parameter model)
    {
        model.ParameterCategory = Model;
    }
}