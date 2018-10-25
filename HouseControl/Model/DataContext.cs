using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Core;
using System.Data.Entity.Validation;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Facade;

namespace Model
{
    
    public class DataContext<TDb> : IContext where TDb : DbContext, IInitable
    {
        private readonly StringBuilder _lastLog = new StringBuilder();
        private ILog _log;
        private ISettings _settings;
        private ITimerSerivce _timerService;
        private readonly object _contextLock = new object();
        private TDb _db;

        public DataContext()
        {
            
        }

        public event Action ContextReset = delegate { };

        public IDBModel CreateModel(Type keyModelKey)
        {
            var set = _db.Set(keyModelKey);
            var a = set.Create();
            set.Add(a);
            return (IDBModel)a;
        }

        public T CreateModel<T>() where T : class, IDBModel
        {
            var a = _db.Set<T>().Create();
            _db.Set<T>().Add(a);
            return a;
        }

        public async Task<List<T>> CustomQuery<T>(Func<IQueryable<T>, List<T>> expression) where T : class, IDBModel
        {
            return await InternalQuery(() => expression(_db.Set<T>()));
        }

        public void Delete(IDBModel model)
        {
            var entry = _db.Entry(model);
            if (entry.State != EntityState.Deleted
                && entry.State != EntityState.Detached)
            {
                _db.Set(model.GetType()).Remove(model);
            }
        }

        public TResult ExecuteStoredProcedure<TResult>(string procedureName, params object[] param)
        {
            throw new NotImplementedException();
        }

        public Task<TResult> ExecuteStoredProcedureAsync<TResult>(string procedureName, params object[] param)
        {
            throw new NotImplementedException();
        }

        public T Find<T>(params object[] key) where T : class, IDBModel
        {
            return _db.Set<T>().Find(key);
        }

        public async Task<IEnumerable<KeyValuePair<T, Q>>> JoinQuery<T, Q>(
            Func<IQueryable<T>, IQueryable<Q>, IEnumerable<KeyValuePair<T, Q>>> expression)
            where T : class, IDBModel where Q : class, IDBModel
        {
            return await InternalQuery(() => expression(_db.Set<T>(), _db.Set<Q>()));
        }

        public async Task<IEnumerable<Tuple<T1, T2, T3, T4>>> JoinQuery<T1, T2, T3, T4>(
            Func<IQueryable<T1>, IQueryable<T2>, IQueryable<T3>, IQueryable<T4>, IEnumerable<Tuple<T1, T2, T3, T4>>>
                expression) where T1 : class, IDBModel
            where T2 : class, IDBModel
            where T3 : class, IDBModel
            where T4 : class, IDBModel
        {
            return await InternalQuery(() => expression(_db.Set<T1>(), _db.Set<T2>(), _db.Set<T3>(), _db.Set<T4>()));
        }

        public void Link<TOne, TMany>(
            TOne model,
            long externManyId,
            Expression<Func<TOne, TMany>> linkProp,
            Func<TMany, ICollection<TOne>> backLink = null) where TOne : class, IDBModel
            where TMany : DbModelWithExternRef
        {
            var linkedQuery = QuickFirstOrDefault<TMany>(a => a.ExternId == externManyId);
            linkedQuery.Wait();
            var linked = linkedQuery.Result ?? CreateModel<TMany>();
            linked.ExternId = externManyId;
            Extension.SetterForExpression(linkProp)(model, linked);
            if (backLink == null)
            {
                return;
            }

            var collection = backLink(linked);
            collection?.Add(model);
        }

        public void LogLastOpertaion()
        {
            _log.Log(LogCategory.Data, _lastLog.ToString());
        }

        public TLink ManyToManyLink<TLink, TFirstModel, TSecondModel>(
            Expression<Func<TLink, TFirstModel>> firstModelPath,
            Expression<Func<TLink, TSecondModel>> secondModelPath,
            TFirstModel firtsModel,
            TSecondModel secondModel
        )
            where TLink : class, IDBModel
            where TFirstModel : class, IDBModel
            where TSecondModel : class, IDBModel
        {
            if (firtsModel == null)
            {
                firtsModel = CreateModel<TFirstModel>();
            }

            if (secondModel == null)
            {
                secondModel = CreateModel<TSecondModel>();
            }

            var link = QuickFirstOrDefault(
                firstModelPath
                    .CompareExpression(firtsModel)
                    .AndExpression(secondModelPath, secondModel));
            link.Wait();
            var foundLink = link.Result;
            if (foundLink != null)
            {
                return foundLink;
            }

            var fmsetter = Extension.SetterForExpression(firstModelPath);
            var smsetter = Extension.SetterForExpression(secondModelPath);
            foundLink = _db.Set<TLink>().Create();
            fmsetter(foundLink, firtsModel);
            smsetter(foundLink, secondModel);
            return foundLink;
        }

        public async Task<List<T>> QueryModels<T>(Expression<Func<T, bool>> where)
            where T : class, IDBModel
        {
            return await InternalQuery(
                () =>
                {
                    lock (typeof(T))
                    {
                        return _db.Set<T>().Where(where).ToList();
                    }
                });
        }

        /// <summary>
        ///     Optimization bottle neck using for checking differend ideas for linq iquaryable entityframework etc
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="expression"></param>
        /// <param name="onlyCached"></param>
        /// <returns></returns>
        public async Task<T> QuickFirstOrDefault<T>(
            Expression<Func<T, bool>> expression,
            DBLoadType loadType = DBLoadType.CacheThenDataBase) where T : class, IDBModel
        {
            ///Time x2 for compiled delegate against expression
            var e = expression; //.Compile(); 
            ///Time x2 for using local before search in base
            //lock (typeof(T))
            //{
            //if(loadType==DBLoadType.CacheOnly)
            //    return _db.Set<T>().Local.AsQueryable().FirstOrDefault(e);
            //}

            //if (res != null)
            //{
            //    return res;
            //}

            return await InternalQuery(
                () =>
                {
                    T res = null;

                    lock (typeof(T))
                    {
                        res = _db.Set<T>().Local.AsQueryable().FirstOrDefault(e);
                    }

                    if (res != null || loadType == DBLoadType.CacheOnly)
                    {
                        return res;
                    }
                    lock (typeof(T))
                    {
                        var result = _db.Set<T>().FirstOrDefault(e);
                        if (result != null && State(result) == EntityState.Deleted)
                        {
                            return null;
                        }

                        return result;
                    }
                });
        }

        public void Reset()
        {
            ResetContext();
        }

        public async Task Save(params IDBModel[] models)
        {
            var destContext = Activator.CreateInstance<TDb>();
            destContext.Database.Log = Log;
            var srcToDstMap = new Dictionary<IDBModel, IDBModel>();
            try
            {
                using (destContext)
                {
                    var added = new HashSet<IDBModel>();
                    foreach (var model in models)
                    {
                        var set = destContext.Set(model.GetType());
                        var curModel = await set.FindAsync((model as IHaveID).ID) as IDBModel;
                        if (curModel == null || added.Contains(curModel))
                        {
                            curModel = (IDBModel)Activator.CreateInstance(set.ElementType);
                            added.Add(curModel);
                        }

                        srcToDstMap[model] = curModel;
                    }

                    var types = models.Select(m => m.GetType()).Distinct();
                    foreach (var type in types)
                    {
                        var set = destContext.Set(type);
                        set.AddRange(added.Where(a => a.GetType().IsAssignableFrom(type)));
                        var srcSet = _db.Set(type);
                        srcSet.AddRange(models.Where(a => a.GetType() == type));
                    }

                    foreach (var dbModel in srcToDstMap)
                    {
                        var src = dbModel.Key;
                        var dst = dbModel.Value;
                        ApplyValues(src, _db, dst, destContext, srcToDstMap);
                    }

                    destContext.ChangeTracker.DetectChanges();
                    destContext.SaveChanges();
                    SyncSourceContext(srcToDstMap, destContext);
                }
            }
            catch (Exception e)
            {
                _log.Log(LogCategory.Data, e);
                throw;
            }
        }

        public async Task<bool> SaveContext(bool resetContext = false)
        {
            return await InternalQuery(() => SaveContextInternal(resetContext));
        }

        public bool SaveContextImmediate(bool resetContext = false)
        {
            return SaveContextInternal(resetContext);
        }

        public EntityState State(IDBModel model)
        {
            return _db.Entry(model).State;
        }

        public async Task Update(IDBModel model)
        {
            await InternalQuery(
                () =>
                {
                    var entry = _db.Entry(model);
                    if (entry.State != EntityState.Detached)
                    {
                        entry.Reload();
                    }
                    return true;
                });
            //LogLastOpertaion();
        }

        public void UpdateImmediate(IDBModel model)
        {
            throw new NotImplementedException();
        }

        IQueryable<T> IContext.Query<T>(Expression<Func<T, bool>> where)
        {
            throw new NotImplementedException();
        }

        private void ApplyValues(
            IDBModel src,
            DbContext srcContext,
            IDBModel dst,
            DbContext dstContext,
            Dictionary<IDBModel, IDBModel> srcToDstMap)
        {
            var sourceEntry = srcContext.Entry(src);
            var destEntry = dstContext.Entry(dst);
            destEntry.CurrentValues.SetValues(sourceEntry.CurrentValues);
            var t = src.GetType().IsInstanceOfType(dst) ? src.GetType() : dst.GetType();
            var links = t.GetProperties().Where(a => typeof(IHaveID).IsAssignableFrom(a.PropertyType));
            foreach (var link in links)
            {
                if (link.GetValue(src) is IHaveID curVal)
                {
                    var linkdst = srcToDstMap.ContainsKey(curVal)
                        ? srcToDstMap[curVal]
                        : dstContext.Set(curVal.GetType()).Find(curVal.ID);
                    link.SetValue(dst, linkdst);
                }
            }
        }

        private void ExeuteQuery<T>(Func<T> t, AsyncResult<T> res)
        {
            var retriesCount = 3;
            for (var i = 0; i < retriesCount; i++)
            {
                try
                {
                    lock (_contextLock)
                    {
                        res.Result = t();
                    }
                    res.Complete = true;
                    return;
                }
                catch (EntityException e)
                {
                    _log.Log(LogCategory.Data, e);
                    LogLastOpertaion();
                    ResetContext();
                }
                catch (Exception e)
                {
                    _log.Log(LogCategory.Data, e);
                    throw;
                }
            }
        }

        private void InitDb()
        {
            _db = Activator.CreateInstance<TDb>();
            _db.Configuration.LazyLoadingEnabled = true;
            _db.Configuration.ProxyCreationEnabled = true;
            _db.Configuration.AutoDetectChangesEnabled = false;
            _db.Database.Log = Log;
            _log.Log(LogCategory.Debug, $"init db '{typeof(TDb)}' ");
            try
            {
                _db.Init();
            }
            catch (Exception e)
            {
                _log.Log(LogCategory.Debug, e);
                _log.Log(LogCategory.Debug, _db.Configuration.ToString());
                throw;
            }
        }

        private Task<T> InternalQuery<T>(Func<T> t)
        {
            var wh = new ManualResetEvent(false);
            var res = new AsyncResult<T>();
            var task = Task.Run(
                () =>
                {
                    if (!res.Complete)
                    {
                        wh.WaitOne(-1);
                    }
                    return res.Result;
                });
            _timerService.Subscribe(
                this,
                () =>
                {
                    try
                    {
                        ExeuteQuery(t, res);
                    }
                    catch (Exception e)
                    {
                        _log.Log(LogCategory.Data, e);
                        Console.WriteLine(e);
                        throw;
                    }
                    finally
                    {
                        wh.Set();
                    }
                },
                0);

            return task;
        }

        private void Log(string s)
        {
            if (s.Contains("Opened connection"))
            {
                _lastLog.Clear();
            }

            _lastLog.Append(s);
            if (_settings.LogLevel >= 2)
            {
                _log.Log(LogCategory.Data, s);
            }
        }

        private void PrepareSourceModel(IDBModel model)
        {
            if (State(model) == EntityState.Detached)
            {
                _db.Set(model.GetType()).Attach(model);
                _db.Set(model.GetType()).Add(model);
            }
        }

        private void ResetContext()
        {
            lock (_contextLock)
            {
                _timerService.Reset();
                _db.Dispose();
                InitDb();
                ContextReset();
            }
        }

        private bool SaveContextInternal(bool resetContext)
        {
            try
            {
                _db.ChangeTracker.DetectChanges();
                _db.SaveChanges();
                if (resetContext)
                {
                    ResetContext();
                }
            }
            catch (DbEntityValidationException e)
            {
                throw new FormattedDbEntityValidationException(e);
            }
            catch (Exception e)
            {
                _log.Log(LogCategory.Data, _lastLog.ToString());
                _log.Log(LogCategory.Data, e);
                throw new EntityException("Ошибка при попытке сохранения контекста", e);
                //return false;
            }

            return true;
        }

        private void SyncSourceContext(Dictionary<IDBModel, IDBModel> srcToDest, TDb destContext)
        {
            foreach (var model in srcToDest)
            {
                try
                {
                    var src = _db.Entry(model.Key);
                    var dst = destContext.Entry(model.Value);
                    src.CurrentValues.SetValues(dst.CurrentValues);
                    src.State = EntityState.Modified;
                    //var s = _db.Entry(dbModel);
                    //if (s.State == EntityState.Added)
                    //{
                    //   _db.Set(dbModel.GetType()).Remove(dbModel);
                    //}

                    //if (s.State == EntityState.Modified || s.State == EntityState.Unchanged)
                    //{
                    //    s.Reload();
                    //}
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    throw;
                }
            }
        }

        private class AsyncResult<T>
        {
            public bool Complete { get; set; }
            public T Result { get; set; }
        }

        public void SetContainer(IServiceContainer container)
        {
            _log = container.Use<ILog>();
            _timerService = container.Use<ITimerSerivce>();
            _settings = container.Use<ISettings>();
            InitDb();

        }
    }

    public class FormattedDbEntityValidationException : Exception
    {
        public FormattedDbEntityValidationException(DbEntityValidationException innerException) :
            base(null, innerException)
        {
        }

        public override string Message
        {
            get
            {
                var innerException = InnerException as DbEntityValidationException;
                if (innerException != null)
                {
                    var sb = new StringBuilder();

                    sb.AppendLine();
                    sb.AppendLine();
                    foreach (var eve in innerException.EntityValidationErrors)
                    {
                        sb.AppendLine(
                            string.Format(
                                "- Entity of type \"{0}\" in state \"{1}\" has the following validation errors:",
                                eve.Entry.Entity.GetType().FullName,
                                eve.Entry.State));
                        foreach (var ve in eve.ValidationErrors)
                        {
                            sb.AppendLine(
                                string.Format(
                                    "-- Property: \"{0}\", Value: \"{1}\", Error: \"{2}\"",
                                    ve.PropertyName,
                                    eve.Entry.CurrentValues.GetValue<object>(ve.PropertyName),
                                    ve.ErrorMessage));
                        }
                    }

                    sb.AppendLine();

                    return sb.ToString();
                }

                return base.Message;
            }
        }
    }
}