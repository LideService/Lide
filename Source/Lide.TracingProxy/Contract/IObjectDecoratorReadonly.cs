using Lide.TracingProxy.Model;

namespace Lide.TracingProxy.Contract
{
    public interface IObjectDecoratorReadonly : IObjectDecorator
    {
        /// <note>There is no thread safety, as the same method of the same object can be called multiple times.</note>
        void ExecuteBeforeInvoke(MethodMetadata methodMetadata)
        {
        }

        /// <note>There is no thread safety, as the same method of the same object can be called multiple times.</note>
        void ExecuteAfterResult(MethodMetadata methodMetadata)
        {
        }
    }
}