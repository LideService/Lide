using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.ExceptionServices;
using System.Threading.Tasks;

namespace Lide.AsyncProxy.DispatchProxyGeneratorAsync
{
    public static class ProxyGeneratorAsync
    {
        private static readonly Dictionary<Type, Dictionary<Type, Type>> BaseTypeAndInterfaceToGeneratedProxyType = new ();
        private static readonly ProxyAssembly ProxyAssembly = new ();
        private static readonly MethodInfo DispatchProxyInvokeMethod = typeof(DispatchProxyAsync).GetTypeInfo().GetDeclaredMethod(nameof(DispatchProxyAsync.Invoke));
        private static readonly MethodInfo DispatchProxyInvokeAsyncMethod = typeof(DispatchProxyAsync).GetTypeInfo().GetDeclaredMethod(nameof(DispatchProxyAsync.InvokeAsync));
        private static readonly MethodInfo DispatchProxyInvokeAsyncTMethod = typeof(DispatchProxyAsync).GetTypeInfo().GetDeclaredMethod(nameof(DispatchProxyAsync.InvokeAsyncT));

        public static object CreateProxyInstance(Type interfaceType, Type baseType)
        {
            var proxiedType = GetProxyType(baseType, interfaceType);
            return Activator.CreateInstance(proxiedType, new DispatchProxyHandlerAsync());
        }

        public static object Invoke(object[] args)
        {
            try
            {
                var context = Resolve(args);
                return DispatchProxyInvokeMethod.Invoke(context.Packed.DispatchProxy, new object[] { context.Method, context.Packed.GetArgs() });
            }
            catch (Exception e)
            {
                ExceptionDispatchInfo.Capture(e).Throw();
                throw;
            }
        }

        public static Task InvokeAsync(object[] args)
        {
            try
            {
                var context = Resolve(args);
                return (Task)DispatchProxyInvokeAsyncMethod.Invoke(context.Packed.DispatchProxy, new object[] { context.Method, context.Packed.GetArgs() });
            }
            catch (Exception e)
            {
                ExceptionDispatchInfo.Capture(e).Throw();
                throw;
            }
        }

        public static Task<T> InvokeAsync<T>(object[] args)
        {
            try
            {
                var context = Resolve(args);
                var genericMethod = DispatchProxyInvokeAsyncTMethod.MakeGenericMethod(typeof(T));
                return (Task<T>)genericMethod.Invoke(context.Packed.DispatchProxy, new object[] { context.Method, context.Packed.GetArgs() });
            }
            catch (Exception tie)
            {
                ExceptionDispatchInfo.Capture(tie).Throw();
                throw;
            }
        }

        private static Type GetProxyType(Type baseType, Type interfaceType)
        {
            lock (BaseTypeAndInterfaceToGeneratedProxyType)
            {
                if (!BaseTypeAndInterfaceToGeneratedProxyType.TryGetValue(baseType, out var interfaceToProxy))
                {
                    interfaceToProxy = new Dictionary<Type, Type>();
                    BaseTypeAndInterfaceToGeneratedProxyType[baseType] = interfaceToProxy;
                }

                if (!interfaceToProxy.TryGetValue(interfaceType, out var generatedProxy))
                {
                    generatedProxy = GenerateProxyType(baseType, interfaceType);
                    interfaceToProxy[interfaceType] = generatedProxy;
                }

                return generatedProxy;
            }
        }

        private static Type GenerateProxyType(Type baseType, Type interfaceType)
        {
            var baseTypeInfo = baseType.GetTypeInfo();
            if (!interfaceType.GetTypeInfo().IsInterface)
            {
                throw new ArgumentException($"InterfaceType must be interface, {interfaceType.FullName}", nameof(interfaceType));
            }

            if (baseTypeInfo.IsSealed)
            {
                throw new ArgumentException($"BaseType cannot be sealed, {baseTypeInfo.FullName}", nameof(baseType));
            }

            if (baseTypeInfo.IsAbstract)
            {
                throw new ArgumentException($"BaseType cannot be abstract {baseType.FullName}", nameof(baseType));
            }

            if (!baseTypeInfo.DeclaredConstructors.Any(c => c.IsPublic && c.GetParameters().Length == 0))
            {
                throw new ArgumentException($"BaseType must have default ctor {baseType.FullName}", nameof(baseType));
            }

            var proxyBuilder = ProxyAssembly.CreateProxy("generatedProxy", baseType);
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
}