using Facade;
using Model;
using ViewModelBase;

public class ParameterCategoryVm : EntytyObjectVM<ParameterCategory>
{
    public ParameterCategoryVm(IServiceContainer container, Models dataBase, ParameterCategory model) : base(container, dataBase, model)
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