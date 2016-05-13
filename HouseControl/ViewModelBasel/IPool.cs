using System;
using System.Collections.Generic;
using Facade;

namespace ViewModelBase
{
    public interface IPool:IService
    {
        IEnumerable<T> GetViewModels<T>() where T : class,IViewModel;
        IViewModel CreateVM(Type type);
        T CreateVM<T>() where T:IViewModel;
        T GetOrCreateVM<T>(int number) where T:class ,IViewModel;
        IViewModel GetOrCreateVM(Type vmType, int id);
        void RemoveVM<T>(int i);
        void FillPool(Type[] types);
        IEntytyObjectVM GetDBVM(Type type, int id);
        IEntytyObjectVM CreateDBObject(Type type);
        T CreateDBObject<T>();
    }
}