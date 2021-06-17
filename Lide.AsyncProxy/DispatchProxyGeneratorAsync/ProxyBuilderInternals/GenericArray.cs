using System;
using System.Reflection.Emit;

namespace Lide.AsyncProxy.DispatchProxyGeneratorAsync.ProxyBuilderInternals
{
    internal class GenericArray<T>
    {
        private readonly ILGenerator _ilGenerator;
        private readonly LocalBuilder _localBuilder;

        public GenericArray(ILGenerator ilGenerator, int length)
        {
            _ilGenerator = ilGenerator;
            _localBuilder = ilGenerator.DeclareLocal(typeof(T[]));

            ilGenerator.Emit(OpCodes.Ldc_I4, length);
            ilGenerator.Emit(OpCodes.Newarr, typeof(T));
            ilGenerator.Emit(OpCodes.Stloc, _localBuilder);
        }

        public void Load()
        {
            _ilGenerator.Emit(OpCodes.Ldloc, _localBuilder);
        }

        public void Get(int value)
        {
            _ilGenerator.Emit(OpCodes.Ldloc, _localBuilder);
            _ilGenerator.Emit(OpCodes.Ldc_I4, value);
            _ilGenerator.Emit(OpCodes.Ldelem_Ref);
        }

        public void BeginSet(int value)
        {
            _ilGenerator.Emit(OpCodes.Ldloc, _localBuilder);
            _ilGenerator.Emit(OpCodes.Ldc_I4, value);
        }

        public void EndSet(Type stackType)
        {
            ProxyBuilderStatics.Convert(_ilGenerator, stackType, typeof(T), false);
            _ilGenerator.Emit(OpCodes.Stelem_Ref);
        }
    }
}