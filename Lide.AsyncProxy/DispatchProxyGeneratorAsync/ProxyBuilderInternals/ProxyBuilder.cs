using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Threading.Tasks;

namespace Lide.AsyncProxy.DispatchProxyGeneratorAsync.ProxyBuilderInternals
{
    internal class ProxyBuilder
    {
        private const int InvokeActionFieldAndCtorParameterIndex = 0;
        private static readonly MethodInfo DelegateInvoke = typeof(DispatchProxyHandlerAsync).GetMethod("InvokeHandle");
        private static readonly MethodInfo DelegateInvokeAsync = typeof(DispatchProxyHandlerAsync).GetMethod("InvokeAsyncHandle");
        private static readonly MethodInfo DelegateInvokeAsyncT = typeof(DispatchProxyHandlerAsync).GetMethod("InvokeAsyncHandleT");

        private readonly ProxyAssembly _assembly;
        private readonly TypeBuilder _typeBuilder;
        private readonly Type _proxyBaseType;
        private readonly List<FieldBuilder> _fields;

        public ProxyBuilder(ProxyAssembly assembly, TypeBuilder typeBuilder, Type proxyBaseType)
        {
            _assembly = assembly;
            _typeBuilder = typeBuilder;
            _proxyBaseType = proxyBaseType;

            _fields = new List<FieldBuilder>
            {
                typeBuilder.DefineField("_handler", typeof(DispatchProxyHandlerAsync), FieldAttributes.Private),
            };
        }

        public Type CreateType()
        {
            Complete();

            // TODO
            return _typeBuilder.CreateTypeInfo()?.AsType() ??
                   throw new ArgumentNullException();
        }

        // [SuppressMessage("Microsoft.Maintainability", "CA1502", Justification = "Who cares")]
        public void AddInterfaceImpl(Type interfaceType)
        {
            _assembly.EnsureTypeIsVisible(interfaceType);
            _typeBuilder.AddInterfaceImplementation(interfaceType);

            var propertyMap = new Dictionary<MethodInfo, PropertyAccessorInfo>(MethodInfoComparer.Instance);
            foreach (PropertyInfo pi in interfaceType.GetRuntimeProperties())
            {
                var ai = new PropertyAccessorInfo(pi.GetMethod, pi.SetMethod);
                if (pi.GetMethod != null)
                {
                    propertyMap[pi.GetMethod] = ai;
                }

                if (pi.SetMethod != null)
                {
                    propertyMap[pi.SetMethod] = ai;
                }
            }

            var eventMap = new Dictionary<MethodInfo, EventAccessorInfo>(MethodInfoComparer.Instance);
            foreach (EventInfo ei in interfaceType.GetRuntimeEvents())
            {
                var ai = new EventAccessorInfo(ei.AddMethod, ei.RemoveMethod, ei.RaiseMethod);
                if (ei.AddMethod != null)
                {
                    eventMap[ei.AddMethod] = ai;
                }

                if (ei.RemoveMethod != null)
                {
                    eventMap[ei.RemoveMethod] = ai;
                }

                if (ei.RaiseMethod != null)
                {
                    eventMap[ei.RaiseMethod] = ai;
                }
            }

            AddMethodImplementations(interfaceType, propertyMap, eventMap);
            AddPropertyImplementations(interfaceType, propertyMap);
            AddEventImplementations(interfaceType, eventMap);
        }

        private void AddEventImplementations(Type interfaceType, Dictionary<MethodInfo, EventAccessorInfo> eventMap)
        {
            foreach (EventInfo eventInfo in interfaceType.GetRuntimeEvents())
            {
                MethodInfo methodInfo = eventInfo.AddMethod ?? eventInfo.RemoveMethod;
                if (methodInfo == null || eventInfo.EventHandlerType == null)
                {
                    // TODO
                    throw new ArgumentNullException();
                }

                EventAccessorInfo ai = eventMap[methodInfo];
                EventBuilder eb = _typeBuilder.DefineEvent(eventInfo.Name, eventInfo.Attributes, eventInfo.EventHandlerType);
                if (ai.AddMethodBuilder != null)
                {
                    eb.SetAddOnMethod(ai.AddMethodBuilder);
                }

                if (ai.RemoveMethodBuilder != null)
                {
                    eb.SetRemoveOnMethod(ai.RemoveMethodBuilder);
                }

                if (ai.RaiseMethodBuilder != null)
                {
                    eb.SetRaiseMethod(ai.RaiseMethodBuilder);
                }
            }
        }

        private void AddPropertyImplementations(Type interfaceType, Dictionary<MethodInfo, PropertyAccessorInfo> propertyMap)
        {
            foreach (PropertyInfo propertyInfo in interfaceType.GetRuntimeProperties())
            {
                MethodInfo methodInfo = propertyInfo.GetMethod ?? propertyInfo.SetMethod;
                if (methodInfo == null)
                {
                    // TODO
                    throw new ArgumentNullException();
                }

                PropertyAccessorInfo ai = propertyMap[methodInfo];
                Type[] parameterTypes = propertyInfo.GetIndexParameters().Select(p => p.ParameterType).ToArray();
                PropertyBuilder pb = _typeBuilder.DefineProperty(propertyInfo.Name, propertyInfo.Attributes, propertyInfo.PropertyType, parameterTypes);
                if (ai.GetMethodBuilder != null)
                {
                    pb.SetGetMethod(ai.GetMethodBuilder);
                }

                if (ai.SetMethodBuilder != null)
                {
                    pb.SetSetMethod(ai.SetMethodBuilder);
                }
            }
        }

        private void AddMethodImplementations(Type interfaceType, Dictionary<MethodInfo, PropertyAccessorInfo> propertyMap, Dictionary<MethodInfo, EventAccessorInfo> eventMap)
        {
            foreach (MethodInfo methodInfo in interfaceType.GetRuntimeMethods())
            {
                MethodBuilder mdb = AddMethodImpl(methodInfo);
                if (propertyMap.TryGetValue(methodInfo, out PropertyAccessorInfo associatedProperty))
                {
                    if (MethodInfoComparer.Instance.Equals(associatedProperty.InterfaceGetMethod, methodInfo))
                    {
                        associatedProperty.GetMethodBuilder = mdb;
                    }
                    else if (MethodInfoComparer.Instance.Equals(associatedProperty.InterfaceSetMethod, methodInfo))
                    {
                        associatedProperty.SetMethodBuilder = mdb;
                    }
                    else
                    {
                        // TODO
                        throw new ArgumentOutOfRangeException();
                    }
                }

                if (eventMap.TryGetValue(methodInfo, out EventAccessorInfo associatedEvent))
                {
                    if (MethodInfoComparer.Instance.Equals(associatedEvent.InterfaceAddMethod, methodInfo))
                    {
                        associatedEvent.AddMethodBuilder = mdb;
                    }
                    else if (MethodInfoComparer.Instance.Equals(associatedEvent.InterfaceRemoveMethod, methodInfo))
                    {
                        associatedEvent.RemoveMethodBuilder = mdb;
                    }
                    else if (MethodInfoComparer.Instance.Equals(associatedEvent.InterfaceRaiseMethod, methodInfo))
                    {
                        associatedEvent.RaiseMethodBuilder = mdb;
                    }
                    else
                    {
                        // TODO
                        throw new ArgumentOutOfRangeException();
                    }
                }
            }
        }

        private MethodBuilder AddMethodImpl(MethodInfo mi)
        {
            ParameterInfo[] parameters = mi.GetParameters();
            Type[] paramTypes = ProxyBuilderStatics.ParamTypes(parameters, false);

            MethodBuilder mdb = _typeBuilder.DefineMethod(mi.Name, MethodAttributes.Public | MethodAttributes.Virtual, mi.ReturnType, paramTypes);
            if (mi.ContainsGenericParameters)
            {
                Type[] ts = mi.GetGenericArguments();
                string[] ss = new string[ts.Length];
                for (int i = 0; i < ts.Length; i++)
                {
                    ss[i] = ts[i].Name;
                }

                GenericTypeParameterBuilder[] genericParameters = mdb.DefineGenericParameters(ss);
                for (int i = 0; i < genericParameters.Length; i++)
                {
                    genericParameters[i].SetGenericParameterAttributes(ts[i].GetTypeInfo().GenericParameterAttributes);
                }
            }

            ILGenerator il = mdb.GetILGenerator();
            ParametersArray args = new ParametersArray(il, paramTypes);
            il.Emit(OpCodes.Nop);
            GenericArray<object> argsArr = new GenericArray<object>(il, ProxyBuilderStatics.ParamTypes(parameters, true).Length);

            for (int i = 0; i < parameters.Length; i++)
            {
                if (!parameters[i].IsOut)
                {
                    argsArr.BeginSet(i);
                    args.Get(i);
                    argsArr.EndSet(parameters[i].ParameterType);
                }
            }

            GenericArray<object> packedArr = new GenericArray<object>(il, PackedArgs.PackedTypes.Length);
            packedArr.BeginSet(ArgumentPositions.DispatchProxyPosition);
            il.Emit(OpCodes.Ldarg_0);
            packedArr.EndSet(typeof(DispatchProxyAsync));

            // TODO
            MethodInfo typeGetTypeFromHandle = typeof(Type).GetRuntimeMethod("GetTypeFromHandle", new[] { typeof(RuntimeTypeHandle) })
                                               ?? throw new ArgumentNullException();

            _assembly.GetTokenForMethod(mi, out Type declaringType, out int methodToken);
            packedArr.BeginSet(ArgumentPositions.DeclaringTypePosition);
            il.Emit(OpCodes.Ldtoken, declaringType);
            il.Emit(OpCodes.Call, typeGetTypeFromHandle);
            packedArr.EndSet(typeof(object));

            packedArr.BeginSet(ArgumentPositions.MethodTokenPosition);
            il.Emit(OpCodes.Ldc_I4, methodToken);
            packedArr.EndSet(typeof(int));

            packedArr.BeginSet(ArgumentPositions.ArgsPosition);
            argsArr.Load();
            packedArr.EndSet(typeof(object[]));

            if (mi.ContainsGenericParameters)
            {
                packedArr.BeginSet(ArgumentPositions.GenericTypesPosition);
                Type[] genericTypes = mi.GetGenericArguments();
                GenericArray<Type> typeArr = new GenericArray<Type>(il, genericTypes.Length);
                for (int i = 0; i < genericTypes.Length; ++i)
                {
                    typeArr.BeginSet(i);
                    il.Emit(OpCodes.Ldtoken, genericTypes[i]);
                    il.Emit(OpCodes.Call, typeGetTypeFromHandle);
                    typeArr.EndSet(typeof(Type));
                }

                typeArr.Load();
                packedArr.EndSet(typeof(Type[]));
            }

            for (int i = 0; i < parameters.Length; i++)
            {
                if (parameters[i].ParameterType.IsByRef)
                {
                    args.BeginSet(i);
                    argsArr.Get(i);
                    args.EndSet(i, typeof(object));
                }
            }

            MethodInfo invokeMethod = DelegateInvoke;
            if (mi.ReturnType == typeof(Task))
            {
                invokeMethod = DelegateInvokeAsync;
            }

            if (ProxyBuilderStatics.IsGenericTask(mi.ReturnType))
            {
                var returnTypes = mi.ReturnType.GetGenericArguments();
                invokeMethod = DelegateInvokeAsyncT.MakeGenericMethod(returnTypes);
            }

            il.Emit(OpCodes.Ldarg_0);
            il.Emit(OpCodes.Ldfld, _fields[InvokeActionFieldAndCtorParameterIndex]);
            packedArr.Load();
            il.Emit(OpCodes.Callvirt, invokeMethod);
            if (mi.ReturnType != typeof(void))
            {
                ProxyBuilderStatics.Convert(il, typeof(object), mi.ReturnType, false);
            }
            else
            {
                il.Emit(OpCodes.Pop);
            }

            il.Emit(OpCodes.Ret);

            _typeBuilder.DefineMethodOverride(mdb, mi);
            return mdb;
        }

        private void Complete()
        {
            Type[] args = new Type[_fields.Count];
            for (int i = 0; i < args.Length; i++)
            {
                args[i] = _fields[i].FieldType;
            }

            ConstructorBuilder cb = _typeBuilder.DefineConstructor(MethodAttributes.Public, CallingConventions.HasThis, args);
            ILGenerator il = cb.GetILGenerator();

            // TODO
            ConstructorInfo baseCtor = _proxyBaseType.GetTypeInfo().DeclaredConstructors.SingleOrDefault(c => c.IsPublic && c.GetParameters().Length == 0)
                ?? throw new ArgumentNullException();

            il.Emit(OpCodes.Ldarg_0);
            il.Emit(OpCodes.Call, baseCtor);

            // store all the fields
            for (int i = 0; i < args.Length; i++)
            {
                il.Emit(OpCodes.Ldarg_0);
                il.Emit(OpCodes.Ldarg, i + 1);
                il.Emit(OpCodes.Stfld, _fields[i]);
            }

            il.Emit(OpCodes.Ret);
        }
    }
}