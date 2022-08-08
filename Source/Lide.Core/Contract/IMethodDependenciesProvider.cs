using System;
using System.Collections.Generic;
using System.Reflection;

namespace Lide.Core.Contract
{
    public interface IMethodDependenciesProvider
    {
        List<MemberInfo> GetDependentMembers(MethodInfo mi);
        List<Type> GetDependencies(MethodInfo mi);

        List<Type> GetDependencies(MethodBase mi);
    }
}