﻿using Facade;
using Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;

namespace ViewModelBase
{
    //also we may use Pool to store ViewModels 
    public class VMFactory:ServiceBase, IPool
    {
        private readonly Dictionary<Type,List<IViewModel>> _pool=new Dictionary<Type, List<IViewModel>>();
        public IViewModel GetOrCreateVM(Type vmType, int id)
        {
            if (!_pool.ContainsKey(vmType))
            {
                _pool[vmType] = new List<IViewModel>();
            }
            var res = _pool[vmType].FirstOrDefault(a => a.ID == id);
            if (res == null)
            {
                res = CreateVM(vmType);
                res.OnCreate(id);
            }
            return res;

        }

        public void RemoveVM<T>(int i)
        {
            if (!_pool.ContainsKey(typeof (T)))
            {
                return;
            }
            var toRemove = _pool[typeof (T)].FirstOrDefault(a => a.ID == i);
            if(toRemove==null)
                return;
            _pool[typeof (T)].Remove(toRemove);
        }

        public T GetOrCreateVM<T>(int number) where T:class,IViewModel
        {
            return (T)GetOrCreateVM(typeof (T), number);
        }

        static VMFactory()
        {
            var ensureDLLIsCopied = System.Data.Entity.SqlServer.SqlProviderServices.Instance;
        }

        

        public T CreateVM<T>() where T : IViewModel
        {
            return (T)CreateVM(typeof(T));
        }

        public IViewModel CreateVM(Type type) 
        {
            if (!_pool.ContainsKey(type))
            {
                _pool[type] = new List<IViewModel>();
            }
            var res = (IViewModel) Activator.CreateInstance(type, Use<IServiceContainer>());
            _pool[type].Add(res);
            return res;
        }

        public IEnumerable<T> GetViewModels<T>() where T:class,IViewModel
        {
            return !_pool.ContainsKey(typeof(T)) ? Enumerable.Empty<T>() : _pool[typeof(T)].Cast<T>();
        }
      
        public T GetViewModel<T>(int id) where T:class
        {
            return !_pool.ContainsKey(typeof(T)) ? null : (T)_pool[typeof(T)].SingleOrDefault(a=>a.ID==id);
        }
    }

    

    public interface INetworService:IService
    {
        string Ping(string address);
    }

    public class NetworService:ServiceBase,INetworService
    {
        public string Ping(string address)
        {
            try
            {
                var p = new Ping();
                var reply = p.Send(address,100);
                return reply.Status.ToString();
            }
            catch (Exception e)
            {
                
                if (e.InnerException != null)
                    return e.InnerException.Message;
                return e.Message;
            }
            
        }

    }

    public interface IPool:IService
    {
        IEnumerable<T> GetViewModels<T>() where T : class,IViewModel;
        IViewModel CreateVM(Type type);
        T CreateVM<T>() where T:IViewModel;
        T GetOrCreateVM<T>(int number) where T:class ,IViewModel;
        IViewModel GetOrCreateVM(Type vmType, int id);
        void RemoveVM<T>(int i);
    }
}



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
