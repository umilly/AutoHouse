using System;
using System.Collections;
using System.Data.Entity;
using System.Data.Entity.Core.EntityClient;
using System.Data.Entity.Validation;
using System.Reflection;
using System.Text;
using Facade;
using Model;

namespace ViewModelBase
{
    public abstract class EntityObjectVm<T> : ViewModelBase, IEntityObjectVM where T : class, IHaveID
    {
        public Type EntityType => typeof (T);

        public bool IsFake => Model == null;
        
        protected T Model { get; private set; }

        protected EntityObjectVm(IServiceContainer container,Models dataBase,T model) : base(container)
        {
            Context = dataBase;
            Model = model;            
        }
        public override int ID
        {
            get { return Model.ID; }
            set{}
        }

        public abstract bool Validate();

        public virtual void Delete()
        {
            var model = Context.Entry(Model as T);
            if (model.State!=EntityState.Deleted
                && model.State != EntityState.Detached)
                Context.Set<T>().Remove(Model);
            if (Use<IGlobalParams>().LogDBAddRemove)
            {
                Use<ILog>().Log(LogCategory.Data, $"'{model.GetType()}' id:'{ID}' deleted");
            }
            Use<IPool>().RemoveVM(GetType(),Model);
        }



        public bool CompareModel(IHaveID id)
        {
            return Model == id;
        }

        public bool IsModelActual { get; } = true;

        public virtual void AddedToPool()
        {
            
        }

        public Models Context { get; }

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
                    StringBuilder sb = new StringBuilder();

                    sb.AppendLine();
                    sb.AppendLine();
                    foreach (var eve in innerException.EntityValidationErrors)
                    {
                        sb.AppendLine(
                            string.Format(
                                "- Entity of type \"{0}\" in state \"{1}\" has the following validation errors:",
                                eve.Entry.Entity.GetType().FullName, eve.Entry.State));
                        foreach (var ve in eve.ValidationErrors)
                        {
                            sb.AppendLine(string.Format("-- Property: \"{0}\", Value: \"{1}\", Error: \"{2}\"",
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