using System;
using System.Reflection;

namespace Lide.TracingProxy.Reflection.Contract
{
    public interface IFastMethodInfoCache
    {
        MethodInfoDelegate GetCompiledMethodInfo(Type originalObjectType, MethodInfo methodInfo);
    }
}