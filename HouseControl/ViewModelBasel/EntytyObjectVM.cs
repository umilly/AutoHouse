using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Core.EntityClient;
using System.Data.Entity.Validation;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Windows.Input;
using Facade;
using Model;

namespace ViewModelBase
{
    public abstract class EntytyObjectVM<T> : ViewModelBase, IEntytyObjectVM where T : class, IHaveID
    {
        public Type EntityType
        {
            get { return typeof (T); } 
        }

        protected T Model { get; private set; }

        public EntytyObjectVM(IServiceContainer container,Models dataBase,T model) : base(container)
        {
            Context = dataBase;
            Model = model;
        }

        protected abstract bool Validate();

        public void SaveDB()
        {
            if (!Validate())
                return;
            try
            {
                if (!Context.Set<T>().Contains(Model))
                    Context.Set<T>().Add(Model);
                Context.SaveChanges();
                OnPropertyChanged(()=>ID);
            }
            catch (DbEntityValidationException e)
            {
                var newException = new FormattedDbEntityValidationException(e);
                Use<ILog>().Log(LogCategory.Data,e.ToString() );
                throw newException;
            }
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
