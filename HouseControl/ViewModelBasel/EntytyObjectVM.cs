using System;
using System.Collections;
using System.Data.Entity;
using System.Data.Entity.Core.EntityClient;
using System.Reflection;
using System.Threading.Tasks;
using Facade;
using Model;

namespace ViewModelBase
{
    public abstract class EntityObjectVm<T> : ViewModelBase, IEntityObjectVM<T> where T : class, IHaveID
    {
        public Type EntityType => typeof (T);

        public bool IsFake => Model == null;
        
        protected T Model { get; private set; }

        protected EntityObjectVm(IServiceContainer container,T model) : base(container)
        {
            Context = container.Use<IContext>();
            Model = model;
            OnCreate();
        }

        protected virtual  Task OnCreate()
        {
            return null;
        }

        public override int ID
        {
            get { return Model.ID; }
            set{}
        }

        public abstract bool Validate();

        public virtual void Delete()
        {
            Context.Delete(Model);
            //var model = Context.Entry(Model as T);
            //if (model.State!=EntityState.Deleted
            //    && model.State != EntityState.Detached)
            //    Context.Set<T>().Remove(Model);
            if (Use<IGlobalParams>().LogDBAddRemove)
            {
                Use<ILog>().Log(LogCategory.Data, $"'{Model.GetType()}' id:'{ID}' deleted");
            }
            Use<IPool>().RemoveVM(GetType(),Model);
        }


        public virtual void AddedToPool()
        {
            
        }

        public bool CompareModel(IHaveID id)
        {
            return Model == id;
        }

        public bool IsModelActual { get; } = true;

        

        public IContext Context { get; }

        T IEntityObjectVM<T>.Model => Model;
    }
}