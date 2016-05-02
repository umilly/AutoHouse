using System;
using Facade;

namespace Common
{



    public class ServiceBase : IService, IDisposable
    {
        public IServiceContainer Container { get; protected set; }


        public void SetContainer(IServiceContainer container)
        {
            Container = container;
            OnContainerSet();
        }

        public virtual void OnContainerSet()
        {
        }

        public ServiceBase(IServiceContainer container)
        {
            Container = container;
        }

        public ServiceBase()
        {
        }

        protected T Use<T>() where T : class
        {
            return Container.Use<T>();
        }

        public bool IsDisposed { get; private set; }

        protected virtual void OnDispose()
        {

        }

        public void Dispose()
        {
            if (IsDisposed)
                return;
            OnDispose();
            IsDisposed = true;
        }

        public string GUID { get; private set; }
    }
}