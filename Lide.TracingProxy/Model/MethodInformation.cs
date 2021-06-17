using System;

namespace Lide.TracingProxy.Model
{
    public class MethodInformation
    {
        public int MethodHashCode { get; set; }
        public string MethodName { get; set; } = null!;
        public Type OriginalObjectType { get; set; } = null!;

        public override int GetHashCode()
        {
            unchecked
            {
                int hash = (int)2166136261;
                hash = (hash * 16777619) ^ MethodHashCode;
                hash = (hash * 16777619) ^ MethodName.GetHashCode();
                hash = (hash * 16777619) ^ OriginalObjectType.GetHashCode();
                return hash;
            }
        }

        public override bool Equals(object? obj)
        {
            if (obj == null || !(obj is MethodInformation caller))
            {
                return false;
            }

            return caller.MethodHashCode == MethodHashCode
                && caller.MethodName == MethodName
                && caller.OriginalObjectType == OriginalObjectType;
        }
    }
}