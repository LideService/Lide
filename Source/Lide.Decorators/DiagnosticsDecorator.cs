using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Text;
using Lide.Core.Contract.Facade;
using Lide.Core.Contract.Provider;
using Lide.Core.Model;
using Lide.TracingProxy.Contract;
using Lide.TracingProxy.Model;

namespace Lide.Decorators
{
    public class DiagnosticsDecorator : IObjectDecoratorReadonly
    {
        private static readonly Stopwatch Stopwatch = Stopwatch.StartNew();
        private readonly IFileFacade _fileFacade;
        private readonly ISignatureProvider _signatureProvider;
        private readonly IScopeIdProvider _scopeIdProvider;
        private readonly ConcurrentDictionary<int, long> _executionTimes;
        private readonly string _filePath;

        public DiagnosticsDecorator(
            IFileFacade fileFacade,
            ISignatureProvider signatureProvider,
            IScopeIdProvider scopeIdProvider,
            IPathProvider pathProvider)
        {
            _fileFacade = fileFacade;
            _signatureProvider = signatureProvider;
            _scopeIdProvider = scopeIdProvider;
            _executionTimes = new ConcurrentDictionary<int, long>();
            _filePath = pathProvider.GetDecoratorFilePath(Id, true);
        }

        public string Id { get; } = "Lide.Diagnostic";

        public void ExecuteBeforeInvoke(MethodMetadata methodMetadata)
        {
            var methodHash = methodMetadata.MethodInfo.GetHashCode();
            var parametersHash = methodMetadata.ParametersMetadata.GetOriginalParameters().GetHashCode();
            var signatureHash = HashCode.Combine(methodHash, parametersHash);
            _executionTimes[signatureHash] = Stopwatch.ElapsedTicks;
        }

        public void ExecuteAfterResult(MethodMetadata methodMetadata)
        {
            var methodHash = methodMetadata.MethodInfo.GetHashCode();
            var parametersHash = methodMetadata.ParametersMetadata.GetOriginalParameters().GetHashCode();
            var signatureHash = HashCode.Combine(methodHash, parametersHash);
            long executionTime = -1;

            if (_executionTimes.ContainsKey(signatureHash))
            {
                _executionTimes.TryRemove(signatureHash, out var startTime);
                executionTime = Stopwatch.ElapsedTicks - startTime;
            }

            var scopeId = $"[{_scopeIdProvider.GetRootScopeId()}.{_scopeIdProvider.GetCurrentScopeId()}]";
            var signature = _signatureProvider.GetMethodSignature(methodMetadata.MethodInfo, SignatureOptions.OnlyBaseNamespace);
            var message = $"{scopeId}: [{signature}] took {executionTime} ticks + {Environment.NewLine}";
            var data = Encoding.ASCII.GetBytes(message);
            _fileFacade.WriteNextBatch(_filePath, data);
        }
    }
}