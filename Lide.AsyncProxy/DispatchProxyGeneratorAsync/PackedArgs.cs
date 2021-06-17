using System;
using Lide.AsyncProxy.DispatchProxyGeneratorAsync.ProxyBuilderInternals;

namespace Lide.AsyncProxy.DispatchProxyGeneratorAsync
{
    internal class PackedArgs
    {
        internal static readonly Type[] PackedTypes = new Type[] { typeof(object), typeof(Type), typeof(int), typeof(object[]), typeof(Type[]), typeof(object) };

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
        public object[] Args => (object[])_args[ArgumentPositions.ArgsPosition];
        public Type[] GenericTypes => (Type[])_args[ArgumentPositions.GenericTypesPosition];
        public object ReturnValue { set => _args[ArgumentPositions.ReturnValuePosition] = value; }
    }
}