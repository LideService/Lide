namespace Lide.TracingProxy.Contract;

public interface IObjectDecorator
{
    /// <summary>
    /// Name of the decorator to be identified with (From request header/body/query).
    /// Must be unique across all existing IObjectDecorators.
    /// </summary>
    string Id { get; }
}