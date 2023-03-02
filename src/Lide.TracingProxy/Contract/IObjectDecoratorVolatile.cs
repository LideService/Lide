using Lide.TracingProxy.Model;

namespace Lide.TracingProxy.Contract;

public interface IObjectDecoratorVolatile : IObjectDecorator, IRequestResponseDecorator
{
    /// <note>There is no thread safety, as the same method of the same object can be called multiple times.</note>
    void ExecuteBeforeInvoke(MethodMetadataVolatile methodMetadata)
    {
    }

    /// <note>There is no thread safety, as the same method of the same object can be called multiple times.</note>
    void ExecuteAfterResult(MethodMetadataVolatile methodMetadata)
    {
    }
}