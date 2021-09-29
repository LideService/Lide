using Lide.Core.Model;

namespace Lide.Decorators.Substitute
{
    public interface ISubstituteLoader
    {
        SubstituteBefore GetBefore(long callId, string methodSignature);
        SubstituteAfter GetAfter(long callId);
        void Load(string filePath);
    }
}