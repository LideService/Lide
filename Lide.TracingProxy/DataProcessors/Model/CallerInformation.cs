namespace Lide.TracingProxy.DataProcessors.Model
{
    public class CallerInformation
    {
        public string CallerMethodName { get; init; } = null!;
        public string CallerFileName { get; init; } = null!;
        public int CallerLineNumber { get; init; }

        public override int GetHashCode()
        {
            unchecked
            {
                int hash = (int)2166136261;
                hash = (hash * 16777619) ^ CallerLineNumber;
                hash = (hash * 16777619) ^ CallerMethodName.GetHashCode();
                hash = (hash * 16777619) ^ CallerFileName.GetHashCode();
                return hash;
            }
        }

        public override bool Equals(object? obj)
        {
            if (obj is not CallerInformation caller)
            {
                return false;
            }

            return caller.CallerFileName == CallerFileName
                && caller.CallerLineNumber == CallerLineNumber
                && caller.CallerMethodName == CallerMethodName;
        }
    }
}