using Facade;
using Model;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.Validation;
using System.Linq;
using System.Net;
using System.Runtime.Remoting.Contexts;
using Common;

namespace ViewModelBase
{
    //also we may use Pool to store ViewModels 
    public class VMFactory:ServiceBase, IPool
    {
        private readonly Dictionary<Type,List<IViewModel>> _pool=new Dictionary<Type, List<IViewModel>>();

        private static Models DB;

        public void Init()
        {
            var ensureDLLIsCopied = System.Data.Entity.SqlServer.SqlProviderServices.Instance;
            try
            {
                DB = new Models("local");
            }
            catch (Exception e)
            {
                throw e;
            }
        }
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
            var res = _pool[vmType].FirstOrDefault(a => a.ID == id) ?? CreateVM(vmType);
            return res;
        }

        public IEntytyObjectVM GetDBVM(Type type, IHaveID id)
        {
            if (!_pool.ContainsKey(type))
                return null;
            var res = _pool[type].Cast<IEntytyObjectVM>().FirstOrDefault(a => a.CompareModel(id));
            return res;
        }

        public T GetDBVM<T>(IHaveID id) where T : IEntytyObjectVM
        {
            return (T) GetDBVM(typeof (T), id);
        }

        public T GetDBVM<T>(int id) where T :class, IEntytyObjectVM 
        {
            if (!_pool.ContainsKey(typeof(T)))
                return null;
            var res = _pool[typeof(T)].Cast<IEntytyObjectVM>().FirstOrDefault(a => a.ID==id);
            return res as T;
        }

        public IEntytyObjectVM CreateDBObject(Type type)
        {
            if (!_pool.ContainsKey(type))
                _pool[type]=new List<IViewModel>();
            var newModel = DB.Set(dbVmTypes[type]).Create();
            DB.Set(dbVmTypes[type]).Add(newModel);
            var res = (IEntytyObjectVM)Activator.CreateInstance(type, Container, DB,newModel);
            _pool[type].Add(res);
            res.AddedToPool();
            return res;
        }

        public T CreateDBObject<T>() where T:IEntytyObjectVM
        {
            return (T) CreateDBObject(typeof (T));
        }

        public void FillPool(Type[] types)
        {
            dbVmTypes.Clear();
            foreach (var dbVmType in types)
            {
                var res = (IEntytyObjectVM)Activator.CreateInstance(dbVmType, null,DB,null);
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
            RemoveVM(typeof(T),i);
        }

        public void RemoveVM(Type type, IHaveID model)
        {
            if (!_pool.ContainsKey(type))
            {
                return;
            }
            var toRemove = _pool[type].FirstOrDefault(a => (a as IEntytyObjectVM).CompareModel(model));
            if (toRemove == null)
                return;
            _pool[type].Remove(toRemove);

        }

        public void RemoveVM(Type type, int i)
        {
            if (!_pool.ContainsKey(type))
            {
                return;
            }
            var toRemove = _pool[type].FirstOrDefault(a => a.ID == i);
            if (toRemove == null)
                return;
            _pool[type].Remove(toRemove);
        }

        public void SaveDB()
        {
            try
            {
                DB.SaveChanges();
            }
            catch (Exception e)
            {
                if(e is DbEntityValidationException)
                    e = new FormattedDbEntityValidationException(e as DbEntityValidationException);
                Use<ILog>().Log(LogCategory.Data, $"Validation fail:\r\n {e.Message}");
                Use<IViewService>().ShowMessage("ИЗМЕНЕНИЯ НЕ БЫЛИ СОХРАНЕНЫ.\r\n В конфигурации была обнаружена ошибка, возможно какая-то сущность была настроена не полностью. ");
            }
        }

        public T GetOrCreateVM<T>(int number) where T:class,IViewModel
        {
            return (T)GetOrCreateVM(typeof (T), number);
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