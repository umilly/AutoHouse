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
        private static Dictionary<Type,List<Type>> _derrivedTypes=new Dictionary<Type, List<Type>>();

        private static Models DB;

        public void Init()
        {
            var ensureDLLIsCopied = System.Data.Entity.SqlServer.SqlProviderServices.Instance;
            try
            {
                DB = new Models("");
            }
            catch (Exception e)
            {
                Use<ILog>().Log(LogCategory.Configuration, e, true);
                throw e;
            }
            Use<ITimerSerivce>().Subsctibe(this,TrySave,60000,true);
        }

        private void TrySave()
        {
            SaveDB(false);
            
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
                TryRegisterType(vmType);
            }
            var res = _pool[vmType].FirstOrDefault(a => a.ID == id) ?? CreateVM(vmType);
            return res;
        }

        public IEntytyObjectVM GetDBVM(Type type, IHaveID id)
        {
            if (!_pool.ContainsKey(type))
                return null;
            IEnumerable<IEntytyObjectVM> all = new List<IEntytyObjectVM>();
            foreach (var t in _derrivedTypes[type].Distinct())
            {
                all = all.Union(_pool[t].Cast<IEntytyObjectVM>());
            }
            var res = all.FirstOrDefault(a => a.CompareModel(id));
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
                TryRegisterType(type);
            object newModel;
            lock (DB)
            {
                newModel = DB.Set(dbVmTypes[type]).Create();
                DB.Set(dbVmTypes[type]).Add(newModel);
            }

            if (Use<IGlobalParams>().LogDBAddRemove)
            {
                Use<ILog>().Log(LogCategory.Data, $"new '{type}' created");
            }
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
            TryRegisterType(dbVmType);
            var task= DB.Set(dbVmTypes[dbVmType]).ForEachAsync(model=>AddEntityFromDB(dbVmType,model));
            task.Wait();
            foreach (var vm in _pool[dbVmType])
            {
                ((IEntytyObjectVM)vm).AddedToPool();
            }
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

        public void SaveDB(bool showMessage=true)
        {
            var reason = showMessage ? "User" : "Timer";
            Use<ILog>().Log(LogCategory.Data, $"Save by {reason} \r\n ");
            try
            {
                lock (DB)
                {
                    DB.SaveChanges();
                }
            }
            catch (Exception e)
            {
                Use<ILog>().Log(LogCategory.Data, $"Save FAILED:\r\n ");

                var addNote = "";
                if (e is DbEntityValidationException)
                {
                    e = new FormattedDbEntityValidationException(e as DbEntityValidationException);
                    addNote = e.Message;
                }
                Use<ILog>().Log(LogCategory.Data, $"Validation fail:\r\n ");
                Use<ILog>().Log(LogCategory.Data, e);
                if (showMessage)
                {
                    Use<IViewService>().ShowMessage(
                        $"ИЗМЕНЕНИЯ НЕ БЫЛИ СОХРАНЕНЫ.\r\n В конфигурации была обнаружена ошибка, возможно какая-то сущность была настроена не полностью.\r\n{addNote} "
                    );
                }
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
                TryRegisterType(type);
            }
            var res = (IViewModel) Activator.CreateInstance(type, Use<IServiceContainer>());
            _pool[type].Add(res);
            return res;
        }

        private void TryRegisterType(Type type)
        {
            if (!_derrivedTypes.ContainsKey(type))
            {
                _derrivedTypes[type]=new List<Type>();
            }

            foreach (var registredType in _derrivedTypes.Keys)
            {
                if(type.IsAssignableFrom(registredType))
                    _derrivedTypes[type].Add(registredType);
                if(registredType.IsAssignableFrom(type))
                    _derrivedTypes[registredType].Add(type);
            }
            if(!_pool.ContainsKey(type))
                _pool[type] = new List<IViewModel>();
        }

        public IEnumerable<T> GetViewModels<T>() where T:class,IViewModel
        {
            var t = typeof(T);
             if (!_derrivedTypes.ContainsKey(t))
            {
                TryRegisterType(t);
            }
            var haveSome = _pool.ContainsKey(t) &&_pool[t].Any();
            if (haveSome)
            {
                foreach (var vm in _pool[t].Cast<T>())
                {
                    yield return vm;
                }
            }
            else
            {
                foreach (var type in _derrivedTypes[t])
                {
                    foreach (var key in _pool[type])
                    {
                        yield return key as T;
                    }
                }
            }
        }
      
        public T GetViewModel<T>(int id) where T:class, IViewModel
        {
            return GetViewModels<T>().SingleOrDefault(a=>a.ID==id);
        }
    }
}