using Facade;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Model;
using ViewModelBase.Annotations;

namespace ViewModelBase
{
    public abstract class ViewModelBase : IViewModel, INotifyPropertyChanged
    {
        abstract public int ID { get; }

        private IServiceContainer _container { get; set; }
        public ViewModelBase(IServiceContainer container)
        { 
            _container=container;
        }

        protected T Use<T>() where T : class,IService
        {
            return _container.Use<T>();
        }
       
        public Models Context { get; private set; }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            if(PropertyChanged!=null)
                PropertyChanged.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
