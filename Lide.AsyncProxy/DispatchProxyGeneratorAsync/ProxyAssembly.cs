using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Threading;
using Lide.AsyncProxy.DispatchProxyGeneratorAsync.ProxyBuilderInternals;

namespace Lide.AsyncProxy.DispatchProxyGeneratorAsync
{
    public class ProxyAssembly
    {
        private readonly AssemblyBuilder _assemblyBuilder;
        private readonly ModuleBuilder _moduleBuilder;

        private readonly Dictionary<MethodBase, int> _methodToToken = new ();
        private readonly List<MethodBase> _methodsByToken = new ();
        private readonly HashSet<string> _ignoresAccessAssemblyNames = new ();
        private ConstructorInfo _ignoresAccessChecksToAttributeConstructor;
        private int _typeId;

        public ProxyAssembly()
        {
            AssemblyBuilderAccess access = AssemblyBuilderAccess.Run;
            var assemblyName = new AssemblyName("Lide.AsyncProxy")
            {
                Version = new Version(1, 0, 0),
            };
            _assemblyBuilder = AssemblyBuilder.DefineDynamicAssembly(assemblyName, access);
            _moduleBuilder = _assemblyBuilder.DefineDynamicModule("Lide.DynamicModule");
        }

        public ProxyBuilder CreateProxy(string name, Type proxyBaseType)
        {
            int nextId = Interlocked.Increment(ref _typeId);
            TypeBuilder tb = _moduleBuilder.DefineType(name + "_" + nextId, TypeAttributes.Public, proxyBaseType);
            return new ProxyBuilder(this, tb, proxyBaseType);
        }

        internal void EnsureTypeIsVisible(Type type)
        {
            TypeInfo typeInfo = type.GetTypeInfo();
            if (!typeInfo.IsVisible)
            {
                string assemblyName = typeInfo.Assembly.GetName().Name;
                if (!_ignoresAccessAssemblyNames.Contains(assemblyName))
                {
                    GenerateInstanceOfIgnoresAccessChecksToAttribute(assemblyName);
                    _ignoresAccessAssemblyNames.Add(assemblyName);
                }
            }
        }

        internal void GetTokenForMethod(MethodBase method, out Type type, out int token)
        {
            type = method.DeclaringType;
            token = 0;
            if (!_methodToToken.TryGetValue(method, out token))
            {
                _methodsByToken.Add(method);
                token = _methodsByToken.Count - 1;
                _methodToToken[method] = token;
            }
        }

        internal MethodBase ResolveMethodToken(int token)
        {
            return _methodsByToken[token];
        }

        private TypeInfo GenerateTypeInfoOfIgnoresAccessChecksToAttribute()
        {
            TypeBuilder attributeTypeBuilder =
                _moduleBuilder.DefineType("System.Runtime.CompilerServices.IgnoresAccessChecksToAttribute",
                    TypeAttributes.Public | TypeAttributes.Class,
                    typeof(Attribute));

            FieldBuilder assemblyNameField = attributeTypeBuilder.DefineField("assemblyName", typeof(string), FieldAttributes.Private);
            ConstructorBuilder constructorBuilder = attributeTypeBuilder.DefineConstructor(MethodAttributes.Public,
                CallingConventions.HasThis,
                new[] { assemblyNameField.FieldType });

            ILGenerator il = constructorBuilder.GetILGenerator();

            il.Emit(OpCodes.Ldarg_0);
            il.Emit(OpCodes.Ldarg, 1);
            il.Emit(OpCodes.Stfld, assemblyNameField);
            il.Emit(OpCodes.Ret);

            attributeTypeBuilder.DefineProperty(
                name: "AssemblyName",
                attributes: PropertyAttributes.None,
                callingConvention: CallingConventions.HasThis,
                returnType: typeof(string),
                parameterTypes: null);

            var getterMethodBuilder = attributeTypeBuilder.DefineMethod(
                name: "get_AssemblyName",
                attributes: MethodAttributes.Public,
                callingConvention: CallingConventions.HasThis,
                returnType: typeof(string),
                parameterTypes: null);

            il = getterMethodBuilder.GetILGenerator();
            il.Emit(OpCodes.Ldarg_0);
            il.Emit(OpCodes.Ldfld, assemblyNameField);
            il.Emit(OpCodes.Ret);

            var attributeUsageTypeInfo = typeof(AttributeUsageAttribute).GetTypeInfo();

            var attributeUsageConstructorInfo =
                attributeUsageTypeInfo.DeclaredConstructors
                    .Single(c => c.GetParameters().Count() == 1 &&
                                 c.GetParameters()[0].ParameterType == typeof(AttributeTargets));

            var allowMultipleProperty =
                attributeUsageTypeInfo.DeclaredProperties
                    .Single(f => string.Equals(f.Name, "AllowMultiple"));

            CustomAttributeBuilder customAttributeBuilder =
                new CustomAttributeBuilder(attributeUsageConstructorInfo,
                    new object[] { AttributeTargets.Assembly },
                    new[] { allowMultipleProperty },
                    new object[] { true });

            attributeTypeBuilder.SetCustomAttribute(customAttributeBuilder);
            return attributeTypeBuilder.CreateTypeInfo();
        }

        private void GenerateInstanceOfIgnoresAccessChecksToAttribute(string assemblyName)
        {
            if (_ignoresAccessChecksToAttributeConstructor == null)
            {
                TypeInfo attributeTypeInfo = GenerateTypeInfoOfIgnoresAccessChecksToAttribute();
                _ignoresAccessChecksToAttributeConstructor = attributeTypeInfo.DeclaredConstructors.Single();
            }

            ConstructorInfo attributeConstructor = _ignoresAccessChecksToAttributeConstructor;
            CustomAttributeBuilder customAttributeBuilder =
                new CustomAttributeBuilder(attributeConstructor, new object[] { assemblyName });
            _assemblyBuilder.SetCustomAttribute(customAttributeBuilder);
        }
    }
}