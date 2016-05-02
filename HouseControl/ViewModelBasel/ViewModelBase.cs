using Facade;
using System.ComponentModel;
using System.Data.Entity.Core.Objects.DataClasses;
using System.Diagnostics.Eventing.Reader;
using System.Runtime.CompilerServices;
using ViewModelBase.Annotations;

namespace ViewModelBase
{
    public abstract class ViewModelBase : IViewModel, INotifyPropertyChanged
    {
        abstract public int ID { get; }
        public virtual void OnCreate(int id)
        {
            
        }

        private IServiceContainer _container { get; set; }
        public ViewModelBase(IServiceContainer container)
        { 
            _container=container;
        }

        protected T Use<T>() where T : class,IService
        {
            return _container.Use<T>();
        }
       
        
        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            if(PropertyChanged!=null)
                PropertyChanged.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

  
}
