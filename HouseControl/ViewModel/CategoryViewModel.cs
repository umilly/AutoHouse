using System;
using System.Collections.Generic;
using System.Linq;
using Facade;
using Model;
using ViewModelBase;

namespace ViewModel
{
    public class CategoryListViewModel:ViewModelBase.ViewModelBase
    {
        public CategoryListViewModel(IServiceContainer container) : base(container)
        {
        }
        public override int ID { get { return -1; }set {} }

        public List<ParameterCategoryVm> Categories
        {
            get { return Use<IPool>().GetViewModels<ParameterCategoryVm>().ToList(); }
        }

        public void CreateCategory()
        {
            var res= Use<IPool>().CreateDBObject<ParameterCategoryVm>();
            res.Name = "Новая категория";
            OnPropertyChanged(()=>Categories);
        }
    }

   
}