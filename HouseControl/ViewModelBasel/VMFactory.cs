using Facade;
using Model;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.Validation;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Runtime.Remoting.Contexts;
using Common;

namespace ViewModelBase
{
    //also we may use Pool to store ViewModels 
    public class VMFactory : IPool
    {
        private static readonly Dictionary<Type, List<Type>> _derrivedTypes = new Dictionary<Type, List<Type>>();
        private static readonly Dictionary<Type, Type> dbVmTypes = new Dictionary<Type, Type>();

        private IServiceContainer _container;
        private IContext _db;
        private ILog _log;
        private readonly Dictionary<Type, IDbSet<IHaveID>> _poolDbVm = new Dictionary<Type, IDbSet<IHaveID>>();
        private readonly Dictionary<Type, Set<IViewModel>> _poolVm = new Dictionary<Type, Set<IViewModel>>();

        public VMFactory()
        {

        }

        public IEntityObjectVM CreateDBObject(Type type)
        {
            if (!_poolDbVm.ContainsKey(type))
            {
                TryRegisterType(type);
            }

            var newModel = _db.CreateModel(dbVmTypes[type]);
            var res = (IEntityObjectVM)Activator.CreateInstance(type, _container,  newModel);
            //var res = (IEntityObjectVM)_container.GetInstance(type, new object[] { newModel });
            AddInternal(type, res);
            res.AddedToPool();
            return res;
        }

        public T CreateDBObject<T>() where T : IEntityObjectVM
        {
            return (T)CreateDBObject(typeof(T));
        }


        public T CreateVM<T>() where T : IViewModel
        {
            return (T)CreateVM(typeof(T));
        }

        public IViewModel CreateVM(Type type)
        {
            if (!_poolDbVm.ContainsKey(type))
            {
                TryRegisterType(type);
            }

            var res = (IViewModel)Activator.CreateInstance(type, _container);

            //var res = (IViewModel)_container.GetInstance(type);
            AddInternal(type, res);
            return res;
        }

        public IEntityObjectVM GetDBVM(Type t, long id)
        {
            return _poolDbVm.ContainsKey(t) ? _poolDbVm[t].Get(id) : null;
        }

        public IEntityObjectVM GetOrCreateDBVM(Type type, IHaveID id)
        {
            if (!_poolDbVm.ContainsKey(type))
            {
                return AddEntityFromDB(type, id).First();
            }

            IEntityObjectVM res = null;

            foreach (var t in _derrivedTypes[type].Distinct())
            {
                res = _poolDbVm[t].Get(id);
                if (res != null)
                {
                    break;
                }
            }

            return res ?? AddEntityFromDB(type, id).First();
        }

        public T GetOrCreateDBVM<T>(IHaveID id) where T : class, IEntityObjectVM
        {
            if (id == null)
                return (T)null;
            return (T)GetOrCreateDBVM(typeof(T), id);
        }

        public IViewModel GetOrCreateVM(Type vmType, decimal id)
        {
            if (typeof(IEntityObjectVM).IsAssignableFrom(vmType))
            {
                throw new ArgumentException("use GetDB or CreateDB for Data Base view models");
            }

            if (!_poolVm.ContainsKey(vmType))
            {
                TryRegisterType(vmType);
            }

            var res = _poolVm[vmType].Find((long)id);
            return res ?? CreateVM(vmType);
        }

        public T GetOrCreateVM<T>(decimal number) where T : class, IViewModel
        {
            return (T)GetOrCreateVM(typeof(T), number);
        }

        public T GetViewModel<T>(long id) where T : class, IViewModel
        {
            var t = typeof(T);
            if (IsDbType(t) && _poolDbVm.ContainsKey(t))
            {
                return _poolDbVm[t].Get(id) as T;
            }
            if (!IsDbType(t) && _poolVm.ContainsKey(t))
            {
                return _poolVm[t].Find(id) as T;
            }
            return GetViewModels<T>().SingleOrDefault(a => a.ID == id);
        }

        public IEnumerable<T> GetViewModels<T>() where T : class, IViewModel
        {
            var res = new List<T>();
            var t = typeof(T);
            if (!_derrivedTypes.ContainsKey(t))
            {
                TryRegisterType(t);
            }

            return IsDbType(t) ? _poolDbVm[t].All.Cast<T>() : _poolVm[t].All.Cast<T>();
            res.AddRange(IsDbType(t) ? _poolDbVm[t].All.Cast<T>() : _poolVm[t].All.Cast<T>());
            return res;
            //TODO Future search derived classes
            //var haveSome = _pool.ContainsKey(t);
            //if (haveSome)
            //{
            //    HashSet<IViewModel> res;
            //    lock (_pool[t])
            //    {
            //        res = _pool[t];
            //    }

            //    foreach (var vm in res.Where(a=>!(a is IEntityObjectVM)||(a as IEntityObjectVM).IsModelActual))
            //    {
            //        yield return (T)vm;
            //    }
            //}

            //foreach (var type in _derrivedTypes[t].Except(new[] {t}))
            //{
            //    HashSet<IViewModel> res;
            //    lock (_pool[type])
            //    {
            //        res = _pool[type];
            //    }

            //    foreach (var key in res.Where(a => !(a is IEntityObjectVM) || (a as IEntityObjectVM).IsModelActual))
            //    {
            //        yield return key as T;
            //    }
            //}
        }

        public void InitByAssambly(Assembly[] assembly)
        {
            var t = typeof(IViewModel);
            var types = assembly.SelectMany(a => a.GetTypes().Where(b => !b.IsAbstract && t.IsAssignableFrom(b)));
            FillMeta(types);
        }

        public void PrepareModels<Tvm, Tm>(IEnumerable<Tm> models)
            where Tvm : IEntityObjectVM<Tm> where Tm : class, IHaveID
        {
            var tvm = typeof(Tvm);
            if (_poolDbVm.ContainsKey(tvm))
            {
                TryRegisterType(tvm);
            }
            IHaveID[] needAdd = models.Where(a => _poolDbVm[tvm].Get(a) == null).ToArray();
            AddEntityFromDB(tvm, needAdd);
        }

        public void RemoveVM<T>(long i)
        {
            RemoveVM(typeof(T), i);
        }

        public void RemoveVM(Type type, IHaveID model)
        {
            RemoveDbVM(type, model);
        }

        public void RemoveVM(Type type, long i)
        {
            if (!_poolVm.ContainsKey(type))
            {
                return;
            }
            _poolVm[type].Remove(i);
            if (!_poolDbVm.ContainsKey(type))
            {
                return;
            }
            _poolDbVm[type].Remove(i);
        }

        public void SaveDB(bool throwError)
        {
            try
            {
                _db.SaveContextImmediate();
            }
            catch (Exception e)
            {
                if (e is DbEntityValidationException)
                {
                    e = new FormattedDbEntityValidationException(e as DbEntityValidationException);
                }
                if (throwError)
                    throw e;
                _log.Log(LogCategory.Data, $"Validation fail:\r\n ");
                _log.Log(LogCategory.Data, e);
                //_log.Log(LogCategory.Data, "ИЗМЕНЕНИЯ НЕ БЫЛИ СОХРАНЕНЫ.\r\n В конфигурации была обнаружена ошибка, возможно какая-то сущность была настроена не полностью. ");
            }
        }

        private IEntityObjectVM[] AddEntityFromDB(Type dbVmType, params IHaveID[] models)
        {
            var res = models.Select(model => (IEntityObjectVM)Activator.CreateInstance(dbVmType, _container, model))
                .ToArray();
            AddInternal(dbVmType, res);
            foreach (var entityObjectVm in res)
            {
                entityObjectVm.AddedToPool();
            }
            return res;
        }

        private void AddInternal(Type type, params IViewModel[] vms)
        {
            if (IsDbType(type))
            {
                _poolDbVm[type].AddRange(vms.Cast<IEntityObjectVM<IHaveID>>());
            }
            else
            {
                _poolVm[type].AddRange(vms);
            }
        }

        private void FillMeta(IEnumerable<Type> types)
        {
            dbVmTypes.Clear();
            foreach (var vmType in types)
            {
                if (typeof(IEntityObjectVM).IsAssignableFrom(vmType))
                {
                    RegisterModelArgInConstructor(vmType);
                }

                //_container.Register(vmType);
                TryRegisterType(vmType);
            }
        }
        private void RegisterModelArgInConstructor(Type vmType)
        {
            Type t = null;
            var curentType = vmType;
            while (t == null)
            {
                if (curentType.IsGenericType && curentType.GetGenericTypeDefinition() == typeof(EntityObjectVm<>))
                {
                    t = curentType;
                }
                else
                {
                    curentType = curentType.BaseType;
                }

                if (curentType == null)
                {
                    break;
                }
            }
            var entityType = curentType.GetGenericArguments()[0];
         
            dbVmTypes[vmType] = entityType;
        }
        private bool IsDbType(Type type)
        {
            return typeof(IEntityObjectVM).IsAssignableFrom(type);
        }

        //private void RegisterModelArgInConstructor(Type vmType)
        //{
        //    Type t = null;
        //    var curentType = vmType;
        //    while (t == null)
        //    {
        //        if (curentType.IsGenericType && curentType.GetGenericTypeDefinition() == typeof(EntityObjectVm<>))
        //        {
        //            t = curentType;
        //        }
        //        else
        //        {
        //            curentType = curentType.BaseType;
        //        }

        //        if (curentType == null)
        //        {
        //            break;
        //        }
        //    }

        //    var entityType = curentType.GetGenericArguments()[0];
        //    _container.RegisterConstructorDependency(
        //        entityType,
        //        (factory, info, arg3)
        //            => arg3.Any() ? arg3[0] : null);
        //    dbVmTypes[vmType] = entityType;
        //}

        private void RemoveDbVM(Type type, IHaveID model)
        {
            if (!_poolDbVm.ContainsKey(type))
            {
                return;
            }
            _poolDbVm[type].Remove(model);
        }

        private void TryRegisterType(Type type)
        {
            if (!_derrivedTypes.ContainsKey(type))
            {
                _derrivedTypes[type] = new List<Type>();
            }

            foreach (var registredType in _derrivedTypes.Keys)
            {
                if (type.IsAssignableFrom(registredType))
                {
                    _derrivedTypes[type].Add(registredType);
                }

                if (registredType.IsAssignableFrom(type))
                {
                    _derrivedTypes[registredType].Add(type);
                }
            }
            if (IsDbType(type) && !_poolDbVm.ContainsKey(type))
            {
                _poolDbVm[type] = new DbSet<IHaveID>();
            }
            if (!_poolVm.ContainsKey(type))
            {
                _poolVm[type] = new Set<IViewModel>();
            }
        }

        public void SetContainer(IServiceContainer container)
        {
            _container = container;
            _db = _container.Use<IContext>();
            _log = _container.Use<ILog>();

        }
    }

    internal interface ISet<in TVm> where TVm : class, IViewModel
    {
        IEnumerable<IViewModel> All { get; }
        void Add(TVm vm);
        IViewModel Find(long id);
        void Remove(long id);
    }

    internal interface IDbSet<TModel> where TModel : class, IHaveID
    {
        IEnumerable<IViewModel> All { get; }
        void Add(IEntityObjectVM<TModel> vm);
        void AddRange(IEnumerable<IEntityObjectVM<TModel>> vms);
        void Clear();
        IEntityObjectVM Get(TModel model);
        IEntityObjectVM Get(long id);
        void Remove(TModel model);
        void Remove(long id);
    }
}