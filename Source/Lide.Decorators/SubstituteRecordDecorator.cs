using System.Reflection;
using Lide.Core.Contract.Facade;
using Lide.Core.Contract.Provider;
using Lide.TracingProxy.Contract;
using Lide.TracingProxy.Model;

namespace Lide.Decorators
{
    public class SubstituteRecordDecorator : IObjectDecoratorReadonly
    {
        private readonly IFileWriter _fileWriter;
        private readonly ICompressionProvider _compressionProvider;
        private readonly ISignatureProvider _signatureProvider;
        private readonly IParametersSerializer _parametersSerializer;

        public SubstituteRecordDecorator(
            IFileWriter fileWriter,
            ICompressionProvider compressionProvider,
            ISignatureProvider signatureProvider,
            IParametersSerializer parametersSerializer)
        {
            _fileWriter = fileWriter;
            _compressionProvider = compressionProvider;
            _signatureProvider = signatureProvider;
            _parametersSerializer = parametersSerializer;
        }

        public string Id { get; } = "Lide.Substitute.Record";
    }
}