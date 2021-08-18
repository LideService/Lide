using System;
using System.Collections.Generic;
using System.Reflection;

namespace Lide.AsyncProxy.DispatchProxyGeneratorAsync.ProxyBuilderInternals
{
    internal sealed class MethodInfoEqualityComparer : EqualityComparer<MethodInfo>
    {
        public static readonly MethodInfoEqualityComparer Instance = new ();

        private MethodInfoEqualityComparer()
        {
        }

        public override bool Equals(MethodInfo left, MethodInfo right)
        {
            if ((left == null && right == null) || ReferenceEquals(left, right))
            {
                return true;
            }

            if (left == null
                || right == null
                || left.DeclaringType != right.DeclaringType
                || left.ReturnType != right.ReturnType
                || left.CallingConvention != right.CallingConvention
                || left.IsStatic != right.IsStatic
                || left.Name != right.Name)
            {
                return false;
            }

            Type[] leftGenericParameters = left.GetGenericArguments();
            Type[] rightGenericParameters = right.GetGenericArguments();
            if (leftGenericParameters.Length != rightGenericParameters.Length)
            {
                return false;
            }

            for (int i = 0; i < leftGenericParameters.Length; i++)
            {
                if (leftGenericParameters[i] != rightGenericParameters[i])
                {
                    return false;
                }
            }

            ParameterInfo[] leftParameters = left.GetParameters();
            ParameterInfo[] rightParameters = right.GetParameters();
            if (leftParameters.Length != rightParameters.Length)
            {
                return false;
            }

            for (var i = 0; i < leftParameters.Length; i++)
            {
                if (leftParameters[i].ParameterType != rightParameters[i].ParameterType)
                {
                    return false;
                }
            }

            return true;
        }

        public override int GetHashCode(MethodInfo obj)
        {
            var hashCode = obj.DeclaringType?.GetHashCode() ?? 0;
            hashCode ^= obj.Name.GetHashCode();
            foreach (var parameter in obj.GetParameters())
            {
                hashCode ^= parameter.ParameterType.GetHashCode();
            }

            return hashCode;
        }
    }
}