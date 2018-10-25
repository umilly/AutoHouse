using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Facade;

namespace Model
{
    public interface IContext:IService
    {
        event Action ContextReset;
        IDBModel CreateModel(Type keyModelKey);
        T CreateModel<T>() where T : class, IDBModel;
        Task<List<T>> CustomQuery<T>(Func<IQueryable<T>, List<T>> expression) where T : class, IDBModel;

        void Delete(IDBModel ebr);
        TResult ExecuteStoredProcedure<TResult>(string procedureName, params object[] param);

        Task<TResult> ExecuteStoredProcedureAsync<TResult>(string procedureName, params object[] param);
        T Find<T>(params object[] key) where T : class, IDBModel;

        Task<IEnumerable<KeyValuePair<T, Q>>> JoinQuery<T, Q>(
            Func<IQueryable<T>, IQueryable<Q>, IEnumerable<KeyValuePair<T, Q>>> expression)
            where T : class, IDBModel where Q : class, IDBModel;

        Task<IEnumerable<Tuple<T1, T2, T3, T4>>> JoinQuery<T1, T2, T3, T4>(
            Func<IQueryable<T1>, IQueryable<T2>, IQueryable<T3>, IQueryable<T4>, IEnumerable<Tuple<T1, T2, T3, T4>>>
                expression)
            where T1 : class, IDBModel
            where T2 : class, IDBModel
            where T3 : class, IDBModel
            where T4 : class, IDBModel;

        void Link<TOne, TMany>(
            TOne model,
            long externId,
            Expression<Func<TOne, TMany>> linkProp,
            Func<TMany, ICollection<TOne>> backLink = null)
            where TOne : class, IDBModel where TMany : DbModelWithExternRef;

        void LogLastOpertaion();

        TLink ManyToManyLink<TLink, TFirstModel, TSecondModel>(
            Expression<Func<TLink, TFirstModel>> firstModelPath,
            Expression<Func<TLink, TSecondModel>> secondModelPath,
            TFirstModel firtsModel,
            TSecondModel secondModel
        )
            where TLink : class, IDBModel
            where TFirstModel : class, IDBModel
            where TSecondModel : class, IDBModel;

        IQueryable<T> Query<T>(Expression<Func<T, bool>> where) where T : class, IDBModel;

        Task<List<T>> QueryModels<T>(Expression<Func<T, bool>> where) where T : class, IDBModel;

        Task<T> QuickFirstOrDefault<T>(
            Expression<Func<T, bool>> expression,
            DBLoadType loadType = DBLoadType.CacheThenDataBase) where T : class, IDBModel;

        void Reset();
        Task Save(params IDBModel[] models);
        Task<bool> SaveContext(bool resetContext = false);

        bool SaveContextImmediate(bool resetContext = false);
        EntityState State(IDBModel model);
        Task Update(IDBModel model);
        void UpdateImmediate(IDBModel model);
    }
    public enum DBLoadType
    {
        CacheThenDataBase,
        CacheOnly
    }
    public abstract class DbModelWithExternRef : DbModel
    {
        [Index(IsUnique = true)]

        public long ExternId { get; set; }
    }
     public abstract class DbModel : IHaveID
    {
        public int ID { get=>Id; }

        [Key]
        public int Id { get; set; }
    }
}