using System;
using System.Reflection.Emit;

namespace Lide.AsyncProxy.DispatchProxyGeneratorAsync.ProxyBuilderInternals
{
    internal class GenericArray<T>
    {
        private readonly ILGenerator _ilGenerator;
        private readonly LocalBuilder _localBuilder;

        public GenericArray(ILGenerator ilGenerator, int len)
        {
            _ilGenerator = ilGenerator;
            _localBuilder = ilGenerator.DeclareLocal(typeof(T[]));

            ilGenerator.Emit(OpCodes.Ldc_I4, len);
            ilGenerator.Emit(OpCodes.Newarr, typeof(T));
            ilGenerator.Emit(OpCodes.Stloc, _localBuilder);
        }

        public void Load()
        {
            _ilGenerator.Emit(OpCodes.Ldloc, _localBuilder);
        }

        public void Get(int i)
        {
            _ilGenerator.Emit(OpCodes.Ldloc, _localBuilder);
            _ilGenerator.Emit(OpCodes.Ldc_I4, i);
            _ilGenerator.Emit(OpCodes.Ldelem_Ref);
        }

        public void BeginSet(int i)
        {
            _ilGenerator.Emit(OpCodes.Ldloc, _localBuilder);
            _ilGenerator.Emit(OpCodes.Ldc_I4, i);
        }

        public void EndSet(Type stackType)
        {
            ProxyBuilderStatics.Convert(_ilGenerator, stackType, typeof(T), false);
            _ilGenerator.Emit(OpCodes.Stelem_Ref);
        }
    }
}