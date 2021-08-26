using System.Reflection;
using System.Threading.Tasks;
using Lide.Core.Contract;
using Lide.Core.Contract.Facade;
using Lide.Core.Contract.Provider;
using Lide.Core.Model;
using Lide.TracingProxy.Contract;
using Lide.TracingProxy.Model;

namespace Lide.Decorators
{
    public class ConsoleDecorator : IObjectDecorator
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
        public bool IsVolatile { get; } = false;

        public object[] ExecuteBeforeInvoke(object plainObject, MethodInfo methodInfo, object[] originalParameters, object[] editedParameters)
        {
            var task = new Task(() =>
            {
                var methodSignature = _signatureProvider.GetMethodSignature(methodInfo, SignatureOptions.OnlyBaseNamespace);
                var parameters = _serializerFacade.Serialize(originalParameters);
                _consoleFacade.WriteLine($"[{_scopeIdProvider.GetScopeId()}] {methodSignature} - {parameters}");
            });
            _taskRunner.AddToQueue(task);

            return originalParameters;
        }

        public ExceptionOrResult ExecuteAfterResult(object plainObject, MethodInfo methodInfo, object[] originalParameters, object[] editedParameters, ExceptionOrResult originalEorR, ExceptionOrResult editedEorR)
        {
            var task = new Task(() =>
            {
                var methodSignature = _signatureProvider.GetMethodSignature(methodInfo, SignatureOptions.OnlyBaseNamespace);
                var parameters = _serializerFacade.Serialize(originalParameters);
                var result = _serializerFacade.Serialize(originalEorR.Result ?? originalEorR.Exception);
                _consoleFacade.WriteLine($"[{_scopeIdProvider.GetScopeId()}] {methodSignature} - {parameters}:{result}");
            });
            _taskRunner.AddToQueue(task);

            return originalEorR;
        }
    }
}