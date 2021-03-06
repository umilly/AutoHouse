﻿using System;
using System.ComponentModel.Design;

namespace Facade
{
    public interface IViewModel
    {
        int ID{get;}
        void OnCreate(int id);
    }
    

    public interface IView
    {
        IViewModel ViewModel { get; set; }
        Type VmType { get; }
    }

    public interface IWrapper
    {

    }
    public interface IViewService:IService
    {
        T CreateView<T>(int id=0) where T : IView;
        IView CreateView(Type type,int Id=0);
        IView NextView { get; set; }
        void ShowMessage(string res);
    }
    public interface IService
    {
        void SetContainer(IServiceContainer container);
    }
    public interface IServiceContainer
    {
        void RegisterType<TInterface, TImplementation>();

        T Use<T>() where T : class;

        void InjectServicesToCustomObject(object customObject);

        void UnRegister<T>(bool skipDispose = false) where T : class;

        string GetServiceNames();

        void UnRegisterAll();
    }
    public interface IHaveID
    {
        int ID { get; }
    }
}
