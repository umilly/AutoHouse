using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Reflection;
using Facade;

namespace ViewModelBase
{
    public interface IPool:IService
    {
        IEntityObjectVM CreateDBObject(Type type);
        T CreateDBObject<T>() where T : IEntityObjectVM;
        IViewModel CreateVM(Type type);

        T CreateVM<T>() where T : IViewModel;


        IEntityObjectVM GetDBVM(Type t, long id);

        IEntityObjectVM GetOrCreateDBVM(Type type, IHaveID id);

        T GetOrCreateDBVM<T>(IHaveID model) where T : class, IEntityObjectVM;
        T GetOrCreateVM<T>(decimal number) where T : class, IViewModel;
        IViewModel GetOrCreateVM(Type vmType, decimal id);
        T GetViewModel<T>(long id) where T : class, IViewModel;
        IEnumerable<T> GetViewModels<T>() where T : class, IViewModel;
        void InitByAssambly(Assembly[] assembly);
        void PrepareModels<Tvm, Tm>(IEnumerable<Tm> models) where Tvm : IEntityObjectVM<Tm> where Tm : class, IHaveID;
        void RemoveVM<T>(long i);
        void RemoveVM(Type type, long i);
        void RemoveVM(Type getType, IHaveID model);

        void SaveDB(bool throwError);
    }
}