using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Linq;
using System.Reflection;
using Facade;
using ViewModelBase;
using IServiceContainer = Facade.IServiceContainer;

public class Container : IServiceContainer
{
    private readonly Dictionary<Type, List<FieldInfo>> _cahedReflrectedFieldServicesForType = new Dictionary<Type, List<FieldInfo>>();
    private readonly Dictionary<Type, Type> _registredTypes = new Dictionary<Type, Type>();
    private readonly Dictionary<Type, object> _registredInstances = new Dictionary<Type, object>();

    public Container()
    {
        RegisterType<IPool, VMFactory>();
        RegisterType<INetworService, NetworService>();
        _registredInstances[typeof (IServiceContainer)] = this;
        FillServiceFieldInfoIfNeed(typeof(Container));
    }

    public void RegisterType<TInterface, TImplementation>()
    {
        _registredTypes[typeof(TInterface)] = typeof(TImplementation);
        FillServiceFieldInfoIfNeed(typeof(TImplementation));
    }

   

    public T Use<T>() where T : class
    {
        Type type = typeof(T);
        return (T)ResolveInternal(type);
    }

    public void InjectServicesToCustomObject(object customObject)
    {
        if (customObject == null)
        {
            return;
        }
        FillServiceFieldInfoIfNeed(customObject.GetType());
        FillServicesForCreatedInternal(customObject, customObject.GetType());
    }

    public void UnRegister<T>(bool skipDispose = false) where T : class
    {
        Type t = typeof(T);
        InternalUnregister(t, skipDispose);
    }

    public string GetServiceNames()
    {
        return string.Join(", ", _registredInstances.Keys.Select(x => x.Name).ToArray());
    }

    public void UnRegisterAll()
    {
        foreach (Type key in _registredInstances.Keys.ToList().Where(x => !_registredTypes.Keys.Contains(x)))
        {
            InternalUnregister(key);
            _registredInstances.Remove(key);
        }
    }

    private object CreateAndRegisterInternal(Type interfaceType, Type instanceType)
    {
        // Инстанциируем
        object result = CreateInstance(instanceType);

        // Сразу регистрируем (разруливаем циклические зависимости)
        _registredInstances.Add(interfaceType, result);

        // Заливаем сервисами
        FillServiceFieldInfoIfNeed(instanceType);
        FillServicesForCreated(result);
        FillServicesForExisted(instanceType, result);

        // Выставляем контейнер
        var service = result as IService;
        if (service != null)
        {
            service.SetContainer(this);
        }

        return result;
    }

    private static object CreateInstance(Type instanceType)
    {
        object result = null;
        result = Activator.CreateInstance(instanceType);
        return result;
    }

    private void FillServicesForExisted(Type newServiceType, object newService)
    {
        Type newType = newServiceType;
        foreach (object service in _registredInstances.Values)
        {
            Type serviceType = service.GetType();
            object existingService = service;

            List<FieldInfo> fields = _cahedReflrectedFieldServicesForType[serviceType];
            fields
                .Where(a => a.FieldType == newType)
                .ForEach(field => field.SetValue(existingService, newService));
        }
    }

    private void FillServicesForCreated(object created)
    {
        Type typeParam = created.GetType();
        FillServicesForCreatedInternal(created, typeParam);
    }

    private void FillServicesForCreatedInternal<T>(T created, Type typeParam)
    {
        foreach (FieldInfo field in _cahedReflrectedFieldServicesForType[typeParam])
        {
            object service = ResolveInternal(field.FieldType);
            if (service == null)
                continue;
            field.SetValue(created, service);
        }
    }

    private void FillServiceFieldInfoIfNeed(Type typeParam)
    {
        if (_cahedReflrectedFieldServicesForType.ContainsKey(typeParam))
            return;
        var fields = typeParam.GetFields(BindingFlags.NonPublic | BindingFlags.Instance);
        _cahedReflrectedFieldServicesForType[typeParam] = new List<FieldInfo>();
        foreach (var field in fields.Where(a => Attribute.IsDefined(a, typeof(InjectAttribute))))
        {
            _cahedReflrectedFieldServicesForType[typeParam].Add(field);
        }
    }

    private object ResolveInternal(Type interfaceType)
    {
        // Сначала проверяем есть ли искомый объект
        if (_registredInstances.ContainsKey(interfaceType))
            return _registredInstances[interfaceType];

        // И если нет, то проверяем нужно ли его создавать
        if (_registredTypes.ContainsKey(interfaceType))
        {
            Type instanceType = _registredTypes[interfaceType];
            return CreateAndRegisterInternal(interfaceType, instanceType);
        }
        throw new ArgumentException(string.Format("service type not registred: {0}",interfaceType));
    }

    private void InternalUnregister(Type t, bool skipDispose = false)
    {
        if (!_registredInstances.ContainsKey(t))
        {
            //D.LogWarning(LoggingTags.Common, string.Format("Try unregister don't existing service {0}", t.Name));
            return;
        }

        object obj = _registredInstances[t];

        if (!skipDispose && (obj is IDisposable))
        {
            (obj as IDisposable).Dispose();
        }

        _registredInstances.Remove(t);

        
    }
}

internal class InjectAttribute:Attribute
{
}




public static class ExtendedEnumerable
{
    public static void ForEach<T>(this IEnumerable<T> all, Action<T> action)
    {
        var reslist = all.ToList();
        foreach (var item in reslist)
        {
            action(item);
        }
    }
}