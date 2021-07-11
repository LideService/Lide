using System;
using Lide.AsyncProxy.DispatchProxyGeneratorAsync.ProxyBuilderInternals;

namespace Lide.AsyncProxy.DispatchProxyGeneratorAsync
{
    internal class PackedArgs
    {
        public static readonly Type[] PackedTypes = { typeof(object), typeof(Type), typeof(int), typeof(object[]), typeof(Type[]), typeof(object) };
        private readonly object[] _args;

        public PackedArgs()
            : this(new object[PackedTypes.Length])
        {
        }

        public PackedArgs(object[] args)
        {
            _args = args;
        }

        public DispatchProxyAsync DispatchProxy => (DispatchProxyAsync)_args[ArgumentPositions.DispatchProxyPosition];
        public Type DeclaringType => (Type)_args[ArgumentPositions.DeclaringTypePosition];
        public int MethodToken => (int)_args[ArgumentPositions.MethodTokenPosition];
        public object[] GetArgs() => (object[])_args[ArgumentPositions.ArgsPosition];
        public Type[] GetGenericTypes() => (Type[])_args[ArgumentPositions.GenericTypesPosition];
        public void SetReturnValue(object value) => _args[ArgumentPositions.ReturnValuePosition] = value;
    }
}