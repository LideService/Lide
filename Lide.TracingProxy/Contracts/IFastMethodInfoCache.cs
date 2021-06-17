using System;
using System.Reflection;

namespace Lide.TracingProxy.Contracts
{
    public interface IFastMethodInfoCache
    {
        MethodInfoDelegate GetCompiledMethodInfo(Type originalObjectType, MethodInfo methodInfo);
    }
}