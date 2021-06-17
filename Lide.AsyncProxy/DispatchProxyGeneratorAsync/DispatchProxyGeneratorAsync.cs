using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.ExceptionServices;
using System.Threading.Tasks;
using Lide.AsyncProxy.DispatchProxyGeneratorAsync.ProxyBuilderInternals;

namespace Lide.AsyncProxy.DispatchProxyGeneratorAsync
{
    internal static class DispatchProxyGeneratorAsync
    {
        private static readonly Dictionary<Type, Dictionary<Type, Type>> BaseTypeAndInterfaceToGeneratedProxyType = new Dictionary<Type, Dictionary<Type, Type>>();

        private static readonly ProxyAssembly ProxyAssembly = new ();
        private static readonly MethodInfo DispatchProxyInvokeMethod = typeof(DispatchProxyAsync).GetTypeInfo().GetDeclaredMethod("Invoke");
        private static readonly MethodInfo DispatchProxyInvokeAsyncMethod = typeof(DispatchProxyAsync).GetTypeInfo().GetDeclaredMethod("InvokeAsync");
        private static readonly MethodInfo DispatchProxyInvokeAsyncTMethod = typeof(DispatchProxyAsync).GetTypeInfo().GetDeclaredMethod("InvokeAsyncT");

        // Returns a new instance of a proxy the derives from 'baseType' and implements 'interfaceType'
        public static object CreateProxyInstance(Type baseType, Type interfaceType)
        {
            Type proxiedType = GetProxyType(baseType, interfaceType);
            return Activator.CreateInstance(proxiedType, new DispatchProxyHandlerAsync());
        }

        public static object Invoke(object[] args)
        {
            try
            {
                var context = Resolve(args);
                object returnValue = DispatchProxyInvokeMethod.Invoke(context.Packed.DispatchProxy, new object[] { context.Method, context.Packed.Args });
                context.Packed.ReturnValue = returnValue;
                return returnValue;
            }
            catch (TargetInvocationException tie)
            {
                ExceptionDispatchInfo.Capture(tie.InnerException!).Throw();
                throw;
            }
        }

        public static Task InvokeAsync(object[] args)
        {
            var context = Resolve(args);
            try
            {
                return (Task)DispatchProxyInvokeAsyncMethod.Invoke(context.Packed.DispatchProxy, new object[] { context.Method, context.Packed.Args });
            }
            catch (TargetInvocationException tie)
            {
                ExceptionDispatchInfo.Capture(tie.InnerException!).Throw();
                throw;
            }
        }

        public static Task<T> InvokeAsync<T>(object[] args)
        {
            var context = Resolve(args);

            try
            {
                var genericMethod = DispatchProxyInvokeAsyncTMethod.MakeGenericMethod(typeof(T));
                return (Task<T>)genericMethod.Invoke(context.Packed.DispatchProxy, new object[] { context.Method, context.Packed.Args });
            }
            catch (TargetInvocationException tie)
            {
                ExceptionDispatchInfo.Capture(tie.InnerException!).Throw();
                throw;
            }
        }

        private static Type GenerateProxyType(Type baseType, Type interfaceType)
        {
            TypeInfo baseTypeInfo = baseType.GetTypeInfo();

            if (!interfaceType.GetTypeInfo().IsInterface)
            {
                throw new ArgumentException($"InterfaceType_Must_Be_Interface, {interfaceType.FullName}", "T");
            }

            if (baseTypeInfo.IsSealed)
            {
                throw new ArgumentException($"BaseType_Cannot_Be_Sealed, {baseTypeInfo.FullName}", "TProxy");
            }

            if (baseTypeInfo.IsAbstract)
            {
                throw new ArgumentException($"BaseType_Cannot_Be_Abstract {baseType.FullName}", "TProxy");
            }

            if (!baseTypeInfo.DeclaredConstructors.Any(c => c.IsPublic && c.GetParameters().Length == 0))
            {
                throw new ArgumentException($"BaseType_Must_Have_Default_Ctor {baseType.FullName}", "TProxy");
            }

            ProxyBuilder pb = ProxyAssembly.CreateProxy("generatedProxy", baseType);
            foreach (Type t in interfaceType.GetTypeInfo().ImplementedInterfaces)
            {
                pb.AddInterfaceImpl(t);
            }

            pb.AddInterfaceImpl(interfaceType);

            Type generatedProxyType = pb.CreateType();
            return generatedProxyType;
        }

        private static ProxyMethodResolverContext Resolve(object[] args)
        {
            PackedArgs packed = new PackedArgs(args);
            MethodBase method = ProxyAssembly.ResolveMethodToken(packed.DeclaringType, packed.MethodToken);
            if (method.IsGenericMethodDefinition)
            {
                method = ((MethodInfo)method).MakeGenericMethod(packed.GenericTypes);
            }

            return new ProxyMethodResolverContext(packed, method);
        }

        private static Type GetProxyType(Type baseType, Type interfaceType)
        {
            lock (BaseTypeAndInterfaceToGeneratedProxyType)
            {
                if (!BaseTypeAndInterfaceToGeneratedProxyType.TryGetValue(baseType, out Dictionary<Type, Type> interfaceToProxy))
                {
                    interfaceToProxy = new Dictionary<Type, Type>();
                    BaseTypeAndInterfaceToGeneratedProxyType[baseType] = interfaceToProxy;
                }

                if (!interfaceToProxy.TryGetValue(interfaceType, out Type generatedProxy))
                {
                    generatedProxy = GenerateProxyType(baseType, interfaceType);
                    interfaceToProxy[interfaceType] = generatedProxy;
                    return generatedProxy;
                }

                return null;
            }
        }
    }
}