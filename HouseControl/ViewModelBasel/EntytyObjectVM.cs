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
    public abstract class EntytyObjectVM<T> : ViewModelBase where T : class, IHaveID
    {
        private static readonly Dictionary<Type, object> _setsByType = new Dictionary<Type, object>();
        private static readonly Dictionary<Type, Func<T>> _createByType = new Dictionary<Type, Func<T>>();
        private static readonly Dictionary<Type, Func<int,T>> _findByType = new Dictionary<Type, Func<int,T>>();
        private static Models DB;

        static EntytyObjectVM()
        {
            try
            {
                DB = new Models("vlad");
            }
            catch (Exception e)
            {

                throw e;
            }

        }

        protected T Model { get; private set; }

        public EntytyObjectVM(IServiceContainer container) : base(container)
        {
            Context = DB;
        }

        public override void OnCreate(int id)
        {
            
            var res= GetEntity(id);
            if(res==null)
            {
                res = CreateNewEntity();
            }
            Model = res;
            base.OnCreate(id);
        }

        private T GetEntity(int id)
        {
            if (!_findByType.ContainsKey(typeof(T)))
                FillDelegates();
            
            return _findByType[typeof(T)](id);
        }

        private T CreateNewEntity()
        {
            if (!_createByType.ContainsKey(typeof (T)))
                FillDelegates();

            return _createByType[typeof (T)]();
        }

        protected abstract bool Validate();

        public void SaveDB()
        {
            if (!Validate())
                return;
            try
            {
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

        private void FillDelegates()
        {
            var set = FindTableSet();
            var createMethod = set.GetType().GetMember("Create").First() as MethodInfo;
            var findMethod = set.GetType().GetMember("Find").First() as MethodInfo;
            var addMethod = set.GetType().GetMember("Add").First() as MethodInfo;
            _createByType[typeof (T)] = () =>
            {
                var res = (T) createMethod.Invoke(set, null);
                addMethod.Invoke(set, new object[] {res});
                return res;
            };
            _findByType[typeof (T)] = (id) =>
            {
                var res = findMethod.Invoke(set, new object[] {new object[]{ id }});
                return res as T ;
            };
        }

        private object FindTableSet()
        {
            if (_setsByType.ContainsKey(typeof (T)))
                return _setsByType[typeof (T)];

            var member = Context.GetType().GetMembers().First(a =>
            {
                if (!(a is PropertyInfo))
                    return false;
                var prop = (a as PropertyInfo);
                if (!prop.PropertyType.GenericTypeArguments.Any())
                    return false;
                return prop.PropertyType.GenericTypeArguments[0] == typeof (T);
            }) as PropertyInfo;
            var set = member.GetValue(Context);
            _setsByType[typeof (T)] = set;
            return set;
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
