using System.Reflection;
using Lide.Core.Contract.Facade;
using Lide.Core.Contract.Provider;
using Lide.TracingProxy.Contract;
using Lide.TracingProxy.Model;

namespace Lide.Decorators
{
    public class ConsoleDecorator : IObjectDecorator
    {
        private readonly IConsoleFacade _consoleFacade;
        private readonly ISignatureProvider _signatureProvider;
        private readonly ISerializerFacade _serializerFacade;

        public ConsoleDecorator(
            IConsoleFacade consoleFacade,
            ISignatureProvider signatureProvider,
            ISerializerFacade serializerFacade)
        {
            _consoleFacade = consoleFacade;
            _signatureProvider = signatureProvider;
            _serializerFacade = serializerFacade;
        }

        public string Id { get; } = "Lide.Console";
        public bool IsVolatile { get; } = false;

        public object[] ExecuteBeforeInvoke(object plainObject, MethodInfo methodInfo, object[] originalParameters, object[] editedParameters)
        {
            var methodSignature = _signatureProvider.GetMethodSignature(methodInfo);
            var parameters = _serializerFacade.Serialize(originalParameters);
            _consoleFacade.WriteLine($"{methodSignature} - {parameters}");

            return originalParameters;
        }

        public ExceptionOrResult ExecuteAfterResult(object plainObject, MethodInfo methodInfo, object[] originalParameters, object[] editedParameters, ExceptionOrResult originalEorR, ExceptionOrResult editedEorR)
        {
            var methodSignature = _signatureProvider.GetMethodSignature(methodInfo);
            var parameters = _serializerFacade.Serialize(originalParameters);
            var result = _serializerFacade.Serialize(originalEorR.Result ?? originalEorR.Exception);
            _consoleFacade.WriteLine($"{methodSignature} - {parameters}:{result}");

            return originalEorR;
        }
    }
}