/* cSpell:disable */
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Threading.Tasks;

namespace Lide.AsyncProxy.DispatchProxyGeneratorAsync.ProxyBuilderInternals;

internal class ProxyBuilder
{
    private const int InvokeActionFieldAndCtorParameterIndex = 0;
    private static readonly MethodInfo DelegateInvoke = typeof(DispatchProxyHandlerAsync).GetMethod(nameof(DispatchProxyHandlerAsync.InvokeHandle));
    private static readonly MethodInfo DelegateInvokeAsync = typeof(DispatchProxyHandlerAsync).GetMethod(nameof(DispatchProxyHandlerAsync.InvokeAsyncHandle));
    private static readonly MethodInfo DelegateInvokeAsyncT = typeof(DispatchProxyHandlerAsync).GetMethod(nameof(DispatchProxyHandlerAsync.InvokeAsyncHandleT));

    private readonly ProxyAssembly _proxyAssembly;
    private readonly TypeBuilder _typeBuilder;
    private readonly Type _proxyBaseType;
    private readonly List<FieldBuilder> _fields;

    public ProxyBuilder(ProxyAssembly proxyAssembly, TypeBuilder typeBuilder, Type proxyBaseType)
    {
        _proxyAssembly = proxyAssembly;
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
        return _typeBuilder.CreateTypeInfo()?.AsType() ?? throw new ArgumentNullException();
    }

    [SuppressMessage("Microsoft.Maintainability", "CA1502", Justification = "It is what it is")]
    public void AddInterfaceImplementation(Type interfaceType)
    {
        _proxyAssembly.EnsureTypeIsVisible(interfaceType);
        _typeBuilder.AddInterfaceImplementation(interfaceType);

        var propertyMap = new Dictionary<MethodInfo, PropertyAccessorInfo>(MethodInfoEqualityComparer.Instance);
        foreach (var pi in interfaceType.GetRuntimeProperties())
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

        var eventMap = new Dictionary<MethodInfo, EventAccessorInfo>(MethodInfoEqualityComparer.Instance);
        foreach (var eventInfo in interfaceType.GetRuntimeEvents())
        {
            var eventAccessorInfo = new EventAccessorInfo(eventInfo.AddMethod, eventInfo.RemoveMethod, eventInfo.RaiseMethod);
            if (eventInfo.AddMethod != null)
            {
                eventMap[eventInfo.AddMethod] = eventAccessorInfo;
            }

            if (eventInfo.RemoveMethod != null)
            {
                eventMap[eventInfo.RemoveMethod] = eventAccessorInfo;
            }

            if (eventInfo.RaiseMethod != null)
            {
                eventMap[eventInfo.RaiseMethod] = eventAccessorInfo;
            }
        }

        foreach (var methodInfo in interfaceType.GetRuntimeMethods())
        {
            MethodBuilder methodBuilder = AddMethodImplementation(methodInfo);
            if (propertyMap.TryGetValue(methodInfo, out var associatedProperty))
            {
                if (MethodInfoEqualityComparer.Instance.Equals(associatedProperty.InterfaceGetMethod, methodInfo))
                {
                    associatedProperty.GetMethodBuilder = methodBuilder;
                }
                else if (MethodInfoEqualityComparer.Instance.Equals(associatedProperty.InterfaceSetMethod, methodInfo))
                {
                    associatedProperty.SetMethodBuilder = methodBuilder;
                }
                else
                {
                    throw new ArgumentOutOfRangeException();
                }
            }

            if (eventMap.TryGetValue(methodInfo, out var associatedEvent))
            {
                if (MethodInfoEqualityComparer.Instance.Equals(associatedEvent.InterfaceAddMethod, methodInfo))
                {
                    associatedEvent.AddMethodBuilder = methodBuilder;
                }
                else if (MethodInfoEqualityComparer.Instance.Equals(associatedEvent.InterfaceRemoveMethod, methodInfo))
                {
                    associatedEvent.RemoveMethodBuilder = methodBuilder;
                }
                else if (MethodInfoEqualityComparer.Instance.Equals(associatedEvent.InterfaceRaiseMethod, methodInfo))
                {
                    associatedEvent.RaiseMethodBuilder = methodBuilder;
                }
                else
                {
                    throw new ArgumentOutOfRangeException();
                }
            }
        }

        foreach (var propertyInfo in interfaceType.GetRuntimeProperties())
        {
            var propertyAccessorInfo = propertyMap[propertyInfo.GetMethod ?? propertyInfo.SetMethod ?? throw new ArgumentNullException()];
            var parameterTypes = propertyInfo.GetIndexParameters().Select(p => p.ParameterType).ToArray();
            var propertyBuilder = _typeBuilder.DefineProperty(propertyInfo.Name, propertyInfo.Attributes, propertyInfo.PropertyType, parameterTypes);
            if (propertyAccessorInfo.GetMethodBuilder != null)
            {
                propertyBuilder.SetGetMethod(propertyAccessorInfo.GetMethodBuilder);
            }

            if (propertyAccessorInfo.SetMethodBuilder != null)
            {
                propertyBuilder.SetSetMethod(propertyAccessorInfo.SetMethodBuilder);
            }
        }

        foreach (var eventInfo in interfaceType.GetRuntimeEvents())
        {
            var eventAccessorInfo = eventMap[eventInfo.AddMethod ?? eventInfo.RemoveMethod ?? throw new ArgumentNullException()];
            var eventBuilder = _typeBuilder.DefineEvent(eventInfo.Name, eventInfo.Attributes, eventInfo.EventHandlerType ?? throw new ArgumentNullException());
            if (eventAccessorInfo.AddMethodBuilder != null)
            {
                eventBuilder.SetAddOnMethod(eventAccessorInfo.AddMethodBuilder);
            }

            if (eventAccessorInfo.RemoveMethodBuilder != null)
            {
                eventBuilder.SetRemoveOnMethod(eventAccessorInfo.RemoveMethodBuilder);
            }

            if (eventAccessorInfo.RaiseMethodBuilder != null)
            {
                eventBuilder.SetRaiseMethod(eventAccessorInfo.RaiseMethodBuilder);
            }
        }
    }

    private void Complete()
    {
        Type[] fieldTypes = new Type[_fields.Count];
        for (int i = 0; i < fieldTypes.Length; i++)
        {
            fieldTypes[i] = _fields[i].FieldType;
        }

        ConstructorBuilder constructorBuilder = _typeBuilder.DefineConstructor(MethodAttributes.Public, CallingConventions.HasThis, fieldTypes);
        ILGenerator ilGenerator = constructorBuilder.GetILGenerator();

        ConstructorInfo baseCtor =
            _proxyBaseType.GetTypeInfo().DeclaredConstructors.SingleOrDefault(c => c.IsPublic && c.GetParameters().Length == 0)
            ?? throw new ArgumentNullException();

        ilGenerator.Emit(OpCodes.Ldarg_0);
        ilGenerator.Emit(OpCodes.Call, baseCtor);

        for (int i = 0; i < fieldTypes.Length; i++)
        {
            ilGenerator.Emit(OpCodes.Ldarg_0);
            ilGenerator.Emit(OpCodes.Ldarg, i + 1);
            ilGenerator.Emit(OpCodes.Stfld, _fields[i]);
        }

        ilGenerator.Emit(OpCodes.Ret);
    }

    private MethodBuilder AddMethodImplementation(MethodInfo mi)
    {
        var parameterInfos = mi.GetParameters();
        var paramTypes = ProxyBuilderStatics.ParamTypes(parameterInfos, false);

        var methodBuilder = _typeBuilder.DefineMethod(mi.Name, MethodAttributes.Public | MethodAttributes.Virtual, mi.ReturnType, paramTypes);
        if (mi.ContainsGenericParameters)
        {
            var genericTypes = mi.GetGenericArguments();
            var stringsArray = new string[genericTypes.Length];
            for (var i = 0; i < genericTypes.Length; i++)
            {
                stringsArray[i] = genericTypes[i].Name;
            }

            var genericTypeParameterBuilders = methodBuilder.DefineGenericParameters(stringsArray);
            for (var i = 0; i < genericTypeParameterBuilders.Length; i++)
            {
                genericTypeParameterBuilders[i].SetGenericParameterAttributes(genericTypes[i].GetTypeInfo().GenericParameterAttributes);
            }
        }

        var ilGenerator = methodBuilder.GetILGenerator();
        var parametersArray = new ParametersArray(ilGenerator, paramTypes);

        ilGenerator.Emit(OpCodes.Nop);
        var argsArr = new GenericArray<object>(ilGenerator, ProxyBuilderStatics.ParamTypes(parameterInfos, true).Length);

        for (int i = 0; i < parameterInfos.Length; i++)
        {
            if (!parameterInfos[i].IsOut)
            {
                argsArr.BeginSet(i);
                parametersArray.Get(i);
                argsArr.EndSet(parameterInfos[i].ParameterType);
            }
        }

        GenericArray<object> packedArr = new(ilGenerator, PackedArgs.PackedTypes.Length);

        packedArr.BeginSet(ArgumentPositions.DispatchProxyPosition);
        ilGenerator.Emit(OpCodes.Ldarg_0);
        packedArr.EndSet(typeof(DispatchProxyAsync));

        var typeGetTypeFromHandle = typeof(Type).GetRuntimeMethod("GetTypeFromHandle", new[] { typeof(RuntimeTypeHandle) }) ?? throw new ArgumentNullException();
        _proxyAssembly.GetTokenForMethod(mi, out var declaringType, out var methodToken);
        packedArr.BeginSet(ArgumentPositions.DeclaringTypePosition);
        ilGenerator.Emit(OpCodes.Ldtoken, declaringType);
        ilGenerator.Emit(OpCodes.Call, typeGetTypeFromHandle);
        packedArr.EndSet(typeof(object));

        packedArr.BeginSet(ArgumentPositions.MethodTokenPosition);
        ilGenerator.Emit(OpCodes.Ldc_I4, methodToken);
        packedArr.EndSet(typeof(int));

        packedArr.BeginSet(ArgumentPositions.ArgsPosition);
        argsArr.Load();
        packedArr.EndSet(typeof(object[]));

        if (mi.ContainsGenericParameters)
        {
            packedArr.BeginSet(ArgumentPositions.GenericTypesPosition);
            var genericTypes = mi.GetGenericArguments();
            var typeArr = new GenericArray<Type>(ilGenerator, genericTypes.Length);
            for (int i = 0; i < genericTypes.Length; ++i)
            {
                typeArr.BeginSet(i);
                ilGenerator.Emit(OpCodes.Ldtoken, genericTypes[i]);
                ilGenerator.Emit(OpCodes.Call, typeGetTypeFromHandle);
                typeArr.EndSet(typeof(Type));
            }

            typeArr.Load();
            packedArr.EndSet(typeof(Type[]));
        }

        for (int i = 0; i < parameterInfos.Length; i++)
        {
            if (parameterInfos[i].ParameterType.IsByRef)
            {
                parametersArray.BeginSet(i);
                argsArr.Get(i);
                parametersArray.EndSet(i, typeof(object));
            }
        }

        var invokeMethod = DelegateInvoke;
        if (mi.ReturnType == typeof(Task))
        {
            invokeMethod = DelegateInvokeAsync;
        }

        if (ProxyBuilderStatics.IsGenericTask(mi.ReturnType))
        {
            var returnTypes = mi.ReturnType.GetGenericArguments();
            invokeMethod = DelegateInvokeAsyncT.MakeGenericMethod(returnTypes);
        }

        ilGenerator.Emit(OpCodes.Ldarg_0);
        ilGenerator.Emit(OpCodes.Ldfld, _fields[InvokeActionFieldAndCtorParameterIndex]);
        packedArr.Load();
        ilGenerator.Emit(OpCodes.Callvirt, invokeMethod);
        if (mi.ReturnType != typeof(void))
        {
            ProxyBuilderStatics.Convert(ilGenerator, typeof(object), mi.ReturnType, false);
        }
        else
        {
            ilGenerator.Emit(OpCodes.Pop);
        }

        ilGenerator.Emit(OpCodes.Ret);
        _typeBuilder.DefineMethodOverride(methodBuilder, mi);
        return methodBuilder;
    }
}