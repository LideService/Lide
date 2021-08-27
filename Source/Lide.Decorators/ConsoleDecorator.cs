using System.Threading.Tasks;
using Lide.Core.Contract;
using Lide.Core.Contract.Facade;
using Lide.Core.Contract.Provider;
using Lide.Core.Model;
using Lide.TracingProxy.Contract;
using Lide.TracingProxy.Model;

namespace Lide.Decorators
{
    public class ConsoleDecorator : IObjectDecoratorReadonly
    {
        private readonly IConsoleFacade _consoleFacade;
        private readonly ISignatureProvider _signatureProvider;
        private readonly ISerializerFacade _serializerFacade;
        private readonly IScopeIdProvider _scopeIdProvider;
        private readonly ITaskRunner _taskRunner;

        public ConsoleDecorator(
            IConsoleFacade consoleFacade,
            ISignatureProvider signatureProvider,
            ISerializerFacade serializerFacade,
            IScopeIdProvider scopeIdProvider,
            ITaskRunner taskRunner)
        {
            _consoleFacade = consoleFacade;
            _signatureProvider = signatureProvider;
            _serializerFacade = serializerFacade;
            _scopeIdProvider = scopeIdProvider;
            _taskRunner = taskRunner;
        }

        public string Id { get; } = "Lide.Console";

        public void ExecuteBeforeInvoke(MethodMetadata methodMetadata)
        {
            var task = new Task(() =>
            {
                var methodInfo = methodMetadata.MethodInfo;
                var editedParameters = methodMetadata.ParametersMetadata.GetEditedParameters();
                var methodSignature = _signatureProvider.GetMethodSignature(methodInfo, SignatureOptions.OnlyBaseNamespace);
                var parameters = _serializerFacade.Serialize(editedParameters);
                _consoleFacade.WriteLine($"[{_scopeIdProvider.GetScopeId()}] {methodSignature} - {parameters}");
            });
            _taskRunner.AddToQueue(task);
        }

        public void ExecuteAfterResult(MethodMetadata methodMetadata)
        {
            // TODO: Accessing result of IEnumerable will enumerate multiple times (possibly).
            // What happens if the result is executable/evaluable?
            var task = new Task(() =>
            {
                var methodInfo = methodMetadata.MethodInfo;
                var editedParameters = methodMetadata.ParametersMetadata.GetEditedParameters();
                var methodSignature = _signatureProvider.GetMethodSignature(methodInfo, SignatureOptions.OnlyBaseNamespace);
                var parameters = _serializerFacade.Serialize(editedParameters);
                var result = _serializerFacade.Serialize(methodMetadata.ReturnMetadata.GetEditedException() ?? methodMetadata.ReturnMetadata.GetEditedResult());
                _consoleFacade.WriteLine($"[{_scopeIdProvider.GetScopeId()}] {methodSignature} - {parameters}:{result}");
            });
            _taskRunner.AddToQueue(task);
        }
    }
}