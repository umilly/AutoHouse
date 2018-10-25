using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using Facade;
using IServiceContainer = Facade.IServiceContainer;

public class Container : IServiceContainer
{
    private readonly Dictionary<Type, List<FieldInfo>> _cahedReflrectedFieldServicesForType = new Dictionary<Type, List<FieldInfo>>();
    private readonly Dictionary<Type, Type> _registredTypes = new Dictionary<Type, Type>();
    private readonly Dictionary<Type, object> _registredInstances = new Dictionary<Type, object>();

    public Container()
    {
        _registredInstances[typeof(IServiceContainer)] = this;
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
        lock (this)
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
            throw new ArgumentException(string.Format("service type not registred: {0}", interfaceType));
        }
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

internal class InjectAttribute : Attribute
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

public static class Extension
{
    private static readonly SHA1CryptoServiceProvider Shaprovider = new SHA1CryptoServiceProvider();

    public static Expression<Func<T, bool>> AndExpression<T, Q>(
        this Expression<Func<T, bool>> left,
        Expression<Func<T, Q>> rigtExpr,
        Q rightVal)
    {
        var param = left.Parameters[0];
        var leftEqualExp = left.Body as BinaryExpression;
        var rightPropName = ((rigtExpr.Body as MemberExpression).Member as PropertyInfo).Name;
        var rightEquelExp = Expression.Equal(Expression.Property(param, rightPropName), Expression.Constant(rightVal));
        var andExpr = Expression.AndAlso(leftEqualExp, rightEquelExp);
        return Expression.Lambda<Func<T, bool>>(andExpr, param);
    }

    public static Expression AndExpressionUnsafe(
        this Expression left,
        Expression rigtExpr)
    {
        var param = (left as LambdaExpression).Parameters[0];
        var leftEqualExp = (left as LambdaExpression).Body as BinaryExpression;
        var rightEquelExp =
            (rigtExpr as LambdaExpression)
            .Body as BinaryExpression;
        var rightParam = (rightEquelExp.Left as MemberExpression).Expression as ParameterExpression;
        if (rightParam == null)
        {
            throw new NotImplementedException("Possibel multiple property, need TODO recursive paramter setter");
        }

        rightEquelExp = Expression.Equal(
            Expression.Property(param, (rightEquelExp.Left as MemberExpression).Member as PropertyInfo),
            rightEquelExp.Right);
        var andExpr = Expression.AndAlso(leftEqualExp, rightEquelExp);
        return Expression.Lambda(andExpr, param);
    }

    public static Expression<Func<T, bool>> CompareExpression<T, Q>(this Expression<Func<T, Q>> left, Q rigt)
    {
        var param = left.Parameters[0];
        var exp = Expression.Equal(
            left.Body as MemberExpression,
            Expression.Constant(rigt));
        var explambda = Expression.Lambda<Func<T, bool>>(exp, param);
        return explambda;
    }

    public static void FullFill(
        this IList source,
        IList newSource)
    {
        var a = new ArrayList(source);
        foreach (var element in a)
        {
            if (!newSource.Contains(element))
            {
                source.Remove(element);
            }
        }

        foreach (var elemtnt in newSource)
        {
            if (!source.Contains(elemtnt))
            {
                source.Add(elemtnt);
            }
        }
    }

    public static Expression<Func<TModel1, TValue>> GetProperty<TModel1, TModel2, TValue>(
        this Expression<Func<TModel1, TModel2>> modelKey,
        Expression<Func<TModel2, TValue>> linkProperty)
    {
        var firstProperty = (modelKey.Body as MemberExpression).Member as PropertyInfo;
        var param = modelKey.Parameters[0];
        var secondProperty = (linkProperty.Body as MemberExpression).Member as PropertyInfo;
        var first = Expression.Property(param, firstProperty);
        var second = Expression.Property(first, secondProperty);
        var res = Expression.Lambda<Func<TModel1, TValue>>(second, param);
        return res;
    }


    public static Func<T, TVal> GetterForExpression<T, TVal>(Expression<Func<T, TVal>> expression)
    {
        return expression.Compile();
    }

    public static string NameOf<T>(this Expression<Func<T>> property)
    {
        return (property.Body as MemberExpression).Member.Name;
    }

    public static string PropertyName<T1, T2>(Expression<Func<T1, T2>> property)
    {
        var prop = (property.Body as MemberExpression).Member as PropertyInfo;
        return prop.Name;
    }

    public static Action<T, TVal> SetterForExpression<T, TVal>(Expression<Func<T, TVal>> expression)
    {
        var prop = (expression.Body as MemberExpression).Member as PropertyInfo;
        var saetter = prop.GetSetMethod();
        return (obj, propVale) => saetter.Invoke(obj, new object[] { propVale });
    }

    public static Action<T1, T2> SetterForName<T1, T2>(string property)
    {
        var set = (typeof(T1).GetMember(property).Single() as PropertyInfo).GetSetMethod();
        var res = new Action<T1, T2>((ob, val) => set.Invoke(ob, new object[] { val }));
        return res;
    }

    public static byte[] SHA(this string src)
    {
        return Shaprovider.ComputeHash(Encoding.ASCII.GetBytes(src ?? ""));
    }

    public static IEnumerable<T> TakeRecursive<T>(this object ob)
    {
        var cache = new HashSet<object>();
        return TakeRecursiveInternal<T>(ob, cache);
    }

    public static bool ToBool(this string value)
    {
        return value == "Y";
    }

    public static string ToOracleBool(this bool value)
    {
        return value ? "Y" : "N";
    }

    private static IEnumerable<T> TakeRecursiveInternal<T>(object ob, HashSet<object> caсhe)
    {
        var result = new List<T>();
        var props = ob.GetType().GetProperties();
        foreach (var prop in props)
        {
            if (typeof(IEnumerable).IsAssignableFrom(prop.PropertyType))
            {
                var items = prop.GetValue(ob) as IEnumerable;
                if (items == null)
                {
                    continue;
                }

                var newItems = items.Cast<object>().Where(a => !caсhe.Contains(a)).ToList();
                newItems.ForEach(a => caсhe.Add(a));
                foreach (var newItem in newItems)
                {
                    if (newItem is T variable)
                    {
                        result.Add(variable);
                    }

                    result.AddRange(TakeRecursiveInternal<T>(newItem, caсhe));
                }
            }
            else
            {
                var item = prop.GetValue(ob);
                if (item != null && !caсhe.Contains(item))
                {
                    caсhe.Add(item);
                    if (item is T i)
                    {
                        result.Add(i);
                    }

                    result.AddRange(TakeRecursiveInternal<T>(item, caсhe));
                }
            }
        }

        return result;
    }
}