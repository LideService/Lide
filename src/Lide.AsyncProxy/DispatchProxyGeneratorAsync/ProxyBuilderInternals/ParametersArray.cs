using System;
using System.Reflection.Emit;

namespace Lide.AsyncProxy.DispatchProxyGeneratorAsync.ProxyBuilderInternals;

internal class ParametersArray
{
    private readonly ILGenerator _ilGenerator;
    private readonly Type[] _paramTypes;

    public ParametersArray(ILGenerator ilGenerator, Type[] paramTypes)
    {
        _ilGenerator = ilGenerator;
        _paramTypes = paramTypes;
    }

    public void Get(int i)
    {
        _ilGenerator.Emit(OpCodes.Ldarg, i + 1);
    }

    public void BeginSet(int i)
    {
        _ilGenerator.Emit(OpCodes.Ldarg, i + 1);
    }

    public void EndSet(int i, Type stackType)
    {
        Type argType = _paramTypes[i].GetElementType();
        ProxyBuilderStatics.Convert(_ilGenerator, stackType, argType, false);
        ProxyBuilderStatics.Stind(_ilGenerator, argType);
    }
}