using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using Facade;

namespace ViewModelBase
{
    public interface IPool:IService
    {
        void Init();
        IEnumerable<T> GetViewModels<T>() where T : class,IViewModel;
        IViewModel CreateVM(Type type);
        T CreateVM<T>() where T:IViewModel;
        T GetOrCreateVM<T>(int number) where T:class ,IViewModel;
        IViewModel GetOrCreateVM(Type vmType, int id);
        void RemoveVM<T>(int i);
        void FillPool(Type[] types);
        IEntytyObjectVM GetDBVM(Type type, IHaveID id);
        IEntytyObjectVM CreateDBObject(Type type);
        T CreateDBObject<T>() where T : IEntytyObjectVM;
        T GetDBVM<T>(IHaveID model) where T : IEntytyObjectVM;
        T GetDBVM<T>(int id) where T : class,IEntytyObjectVM;
        void RemoveVM(Type type, int i);
        void SaveDB(bool showMessage = true);
        void RemoveVM(Type getType, IHaveID model);
    }
}