using Lide.Core.Contract.Facade;
using Lide.Core.Contract.Provider;
using Lide.Core.Model;
using Lide.TracingProxy.Contract;
using Lide.TracingProxy.Model;

namespace Lide.Decorators;

public class ConsoleDecorator : IObjectDecoratorReadonly
{
    private readonly ILoggerFacade _loggerFacade;
    private readonly ISignatureProvider _signatureProvider;
    private readonly IJsonSerializeProvider _jsonSerializeProvider;
    private readonly IScopeIdProvider _scopeIdProvider;

    public ConsoleDecorator(
        ILoggerFacade loggerFacade,
        ISignatureProvider signatureProvider,
        IJsonSerializeProvider jsonSerializeProvider,
        IScopeIdProvider scopeIdProvider)
    {
        _loggerFacade = loggerFacade;
        _signatureProvider = signatureProvider;
        _jsonSerializeProvider = jsonSerializeProvider;
        _scopeIdProvider = scopeIdProvider;
    }

    public string Id => "Lide.Console";

    public void ExecuteBeforeInvoke(MethodMetadata methodMetadata)
    {
        var methodInfo = methodMetadata.MethodInfo;
        var editedParameters = methodMetadata.ParametersMetadata.GetEditedParameters();
        var methodSignature = _signatureProvider.GetMethodSignature(methodInfo, SignatureOptions.OnlyBaseNamespace);
        var parameters = _jsonSerializeProvider.Serialize(editedParameters);
        _loggerFacade.Log($"[{_scopeIdProvider.GetRootScopeId()}][{_scopeIdProvider.GetCurrentScopeId()}] {methodSignature} - {parameters}");
    }

    public void ExecuteAfterResult(MethodMetadata methodMetadata)
    {
        var methodInfo = methodMetadata.MethodInfo;
        var editedParameters = methodMetadata.ParametersMetadata.GetEditedParameters();
        var methodSignature = _signatureProvider.GetMethodSignature(methodInfo, SignatureOptions.OnlyBaseNamespace);
        var parameters = _jsonSerializeProvider.Serialize(editedParameters);
        var result = _jsonSerializeProvider.Serialize(methodMetadata.ReturnMetadata.GetEditedException() ?? methodMetadata.ReturnMetadata.GetEditedResult());
        _loggerFacade.Log($"[{_scopeIdProvider.GetRootScopeId()}][{_scopeIdProvider.GetCurrentScopeId()}] {methodSignature} - {parameters}:{result}");
    }
}