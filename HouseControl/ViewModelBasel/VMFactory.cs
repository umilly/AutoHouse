using Facade;
using Model;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using Common;

namespace ViewModelBase
{
    //also we may use Pool to store ViewModels 
    public class VMFactory:ServiceBase, IPool
    {
        private readonly Dictionary<Type,List<IViewModel>> _pool=new Dictionary<Type, List<IViewModel>>();

        private static Models DB;

        private static Dictionary<Type,Type> dbVmTypes=new Dictionary<Type, Type>();
        public IViewModel GetOrCreateVM(Type vmType, int id)
        {
            if (typeof(IEntytyObjectVM).IsAssignableFrom(vmType))
            {
                throw new ArgumentException("use GetDB or CreateDB for Data Base view models");
            }
            if (!_pool.ContainsKey(vmType))
            {
                _pool[vmType] = new List<IViewModel>();
            }
            var res = _pool[vmType].FirstOrDefault(a => a.ID == id);
            if (res == null)
            {
                res = CreateVM(vmType);
                res.OnCreate(id);
            }
            return res;
        }

        public IEntytyObjectVM GetDBVM(Type type, int id)
        {
            if (!_pool.ContainsKey(type))
                return null;
            var res = _pool[type].FirstOrDefault(a => a.ID == id);
            return res as IEntytyObjectVM;
        }
        public IEntytyObjectVM CreateDBObject(Type type)
        {
            if (!_pool.ContainsKey(type))
                _pool[type]=new List<IViewModel>();
            var newModel = DB.Set(dbVmTypes[type]).Create();
            DB.Set(dbVmTypes[type]).Add(newModel);
            var res = (IEntytyObjectVM)Activator.CreateInstance(type, Container, DB,newModel);
            _pool[type].Add(res);
            return res;
        }

        public T CreateDBObject<T>()
        {
            return (T) CreateDBObject(typeof (T));
        }

        public void FillPool(Type[] types)
        {
            dbVmTypes.Clear();
            foreach (var dbVmType in types)
            {
                var res = (IEntytyObjectVM)Activator.CreateInstance(dbVmType, null,null,null);
                dbVmTypes[dbVmType] = res.EntityType;
                FillType(dbVmType);
            }

        }

        private void FillType(Type dbVmType)
        {
            _pool[dbVmType] = new List<IViewModel>();
            var task= DB.Set(dbVmTypes[dbVmType]).ForEachAsync(model=>AddEntityFromDB(dbVmType,model));
            task.Wait();
        }

        private void AddEntityFromDB(Type dbVmType, object model)
        {
            var res = (IEntytyObjectVM)Activator.CreateInstance(dbVmType, Container, DB, model);
            _pool[dbVmType].Add(res);
        }

        public void RemoveVM<T>(int i)
        {
            if (!_pool.ContainsKey(typeof (T)))
            {
                return;
            }
            var toRemove = _pool[typeof (T)].FirstOrDefault(a => a.ID == i);
            if(toRemove==null)
                return;
            _pool[typeof (T)].Remove(toRemove);
        }

        public T GetOrCreateVM<T>(int number) where T:class,IViewModel
        {
            return (T)GetOrCreateVM(typeof (T), number);
        }

        static VMFactory()
        {
            var ensureDLLIsCopied = System.Data.Entity.SqlServer.SqlProviderServices.Instance;
            try
            {
                DB = new Models("vlad");
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        

        public T CreateVM<T>() where T : IViewModel
        {
            return (T)CreateVM(typeof(T));
        }

        public IViewModel CreateVM(Type type) 
        {
            if (!_pool.ContainsKey(type))
            {
                _pool[type] = new List<IViewModel>();
            }
            var res = (IViewModel) Activator.CreateInstance(type, Use<IServiceContainer>());
            _pool[type].Add(res);
            return res;
        }

        public IEnumerable<T> GetViewModels<T>() where T:class,IViewModel
        {
            return !_pool.ContainsKey(typeof(T)) ? Enumerable.Empty<T>() : _pool[typeof(T)].Cast<T>();
        }
      
        public T GetViewModel<T>(int id) where T:class
        {
            return !_pool.ContainsKey(typeof(T)) ? null : (T)_pool[typeof(T)].SingleOrDefault(a=>a.ID==id);
        }
    }
}