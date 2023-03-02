using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Runtime.ExceptionServices;
using System.Threading.Tasks;

namespace Lide.AsyncProxy.DispatchProxyGeneratorAsync;

internal static class ProxyGeneratorAsync
{
    private static readonly Dictionary<Type, Dictionary<Type, Type>> BaseTypeAndInterfaceToGeneratedProxyType = new();
    private static readonly ProxyAssembly ProxyAssembly = new();
    private static readonly MethodInfo DispatchProxyInvokeMethod = typeof(DispatchProxyAsync).GetTypeInfo().GetDeclaredMethod(nameof(DispatchProxyAsync.Invoke));
    private static readonly MethodInfo DispatchProxyInvokeAsyncMethod = typeof(DispatchProxyAsync).GetTypeInfo().GetDeclaredMethod(nameof(DispatchProxyAsync.InvokeAsync));
    private static readonly MethodInfo DispatchProxyInvokeAsyncTMethod = typeof(DispatchProxyAsync).GetTypeInfo().GetDeclaredMethod(nameof(DispatchProxyAsync.InvokeAsyncT));

    public static object CreateProxyInstance(Type interfaceType, Type proxyType)
    {
        var generatedProxyType = GetProxyType(proxyType, interfaceType);
        return Activator.CreateInstance(generatedProxyType, new DispatchProxyHandlerAsync());
    }

    [DebuggerStepThrough]
    [DebuggerHidden]
    public static object Invoke(object[] args)
    {
        try
        {
            var context = Resolve(args);
            return DispatchProxyInvokeMethod.Invoke(context.Packed.DispatchProxy, new object[] { context.Method, context.Packed.GetArgs() });
        }
        catch (Exception e)
        {
            ExceptionDispatchInfo.Capture(e.InnerException ?? e).Throw();
            throw;
        }
    }

    [DebuggerStepThrough]
    [DebuggerHidden]
    public static Task InvokeAsync(object[] args)
    {
        try
        {
            var context = Resolve(args);
            return (Task)DispatchProxyInvokeAsyncMethod.Invoke(context.Packed.DispatchProxy, new object[] { context.Method, context.Packed.GetArgs() });
        }
        catch (Exception e)
        {
            ExceptionDispatchInfo.Capture(e.InnerException ?? e).Throw();
            throw;
        }
    }

    [DebuggerStepThrough]
    [DebuggerHidden]
    public static Task<T> InvokeAsync<T>(object[] args)
    {
        try
        {
            var context = Resolve(args);
            var genericMethod = DispatchProxyInvokeAsyncTMethod.MakeGenericMethod(typeof(T));
            return (Task<T>)genericMethod.Invoke(context.Packed.DispatchProxy, new object[] { context.Method, context.Packed.GetArgs() });
        }
        catch (Exception e)
        {
            ExceptionDispatchInfo.Capture(e.InnerException ?? e).Throw();
            throw;
        }
    }

    private static Type GetProxyType(Type proxyType, Type interfaceType)
    {
        lock (BaseTypeAndInterfaceToGeneratedProxyType)
        {
            if (!BaseTypeAndInterfaceToGeneratedProxyType.TryGetValue(proxyType, out var interfaceToProxy))
            {
                interfaceToProxy = new Dictionary<Type, Type>();
                BaseTypeAndInterfaceToGeneratedProxyType[proxyType] = interfaceToProxy;
            }

            if (!interfaceToProxy.TryGetValue(interfaceType, out var generatedProxy))
            {
                generatedProxy = GenerateProxyType(proxyType, interfaceType);
                interfaceToProxy[interfaceType] = generatedProxy;
            }

            return generatedProxy;
        }
    }

    private static Type GenerateProxyType(Type proxyType, Type interfaceType)
    {
        var proxyTypeInfo = proxyType.GetTypeInfo();
        if (!interfaceType.GetTypeInfo().IsInterface)
        {
            throw new ArgumentException($"InterfaceType must be interface, {interfaceType.FullName}", nameof(interfaceType));
        }

        if (proxyTypeInfo.IsSealed)
        {
            throw new ArgumentException($"ProxyType cannot be sealed, {proxyTypeInfo.FullName}", nameof(proxyType));
        }

        if (proxyTypeInfo.IsAbstract)
        {
            throw new ArgumentException($"ProxyType cannot be abstract {proxyType.FullName}", nameof(proxyType));
        }

        if (!proxyTypeInfo.DeclaredConstructors.Any(c => c.IsPublic && c.GetParameters().Length == 0))
        {
            throw new ArgumentException($"ProxyType must have default ctor {proxyType.FullName}", nameof(proxyType));
        }

        var proxyBuilder = ProxyAssembly.CreateProxy($"Lide.RuntimeGeneratedTypes.{proxyType.Name}.{interfaceType.Name}", proxyType);
        foreach (var implementedInterfacesTypes in interfaceType.GetTypeInfo().ImplementedInterfaces)
        {
            proxyBuilder.AddInterfaceImplementation(implementedInterfacesTypes);
        }

        proxyBuilder.AddInterfaceImplementation(interfaceType);

        Type generatedProxyType = proxyBuilder.CreateType();
        return generatedProxyType;
    }

    private static ProxyMethodResolverContext Resolve(object[] args)
    {
        var packed = new PackedArgs(args);
        var method = ProxyAssembly.ResolveMethodToken(packed.MethodToken);
        if (method.IsGenericMethodDefinition)
        {
            method = ((MethodInfo)method).MakeGenericMethod(packed.GetGenericTypes());
        }

        return new ProxyMethodResolverContext(packed, method);
    }
}