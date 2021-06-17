using System;
using System.Reflection.Emit;

namespace Lide.AsyncProxy.DispatchProxyGeneratorAsync.ProxyBuilderInternals
{
    internal class ParametersArray
    {
        private readonly ILGenerator _ilGenerator;
        private readonly Type[] _paramTypes;

        public ParametersArray(ILGenerator ilGenerator, Type[] paramTypes)
        {
            _ilGenerator = ilGenerator;
            _paramTypes = paramTypes;
        }

        public void Get(int index)
        {
            _ilGenerator.Emit(OpCodes.Ldarg, index + 1);
        }

        public void BeginSet(int index)
        {
            _ilGenerator.Emit(OpCodes.Ldarg, index + 1);
        }

        public void EndSet(int index, Type stackType)
        {
            Type argType = _paramTypes[index].GetElementType();
            ProxyBuilderStatics.Convert(_ilGenerator, stackType, argType, false);
            ProxyBuilderStatics.Stind(_ilGenerator, argType);
        }
    }
}