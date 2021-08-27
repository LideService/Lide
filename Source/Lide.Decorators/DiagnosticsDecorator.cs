using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Reflection;
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
        private readonly IFileWriter _fileWriter;
        private readonly ISignatureProvider _signatureProvider;
        private readonly IScopeIdProvider _scopeIdProvider;
        private readonly ConcurrentDictionary<int, long> _executionTimes;

        public DiagnosticsDecorator(
            IFileWriter fileWriter,
            ISignatureProvider signatureProvider,
            IScopeIdProvider scopeIdProvider)
        {
            _fileWriter = fileWriter;
            _signatureProvider = signatureProvider;
            _scopeIdProvider = scopeIdProvider;
            _executionTimes = new ConcurrentDictionary<int, long>();
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

            _fileWriter.AddToQueue(() =>
            {
                var scopeId = _scopeIdProvider.GetScopeId();
                var signature = _signatureProvider.GetMethodSignature(methodMetadata.MethodInfo, SignatureOptions.AllSet);
                var message = $"[{scopeId}]: [{signature}] took {executionTime} ticks + {Environment.NewLine}";
                return Encoding.ASCII.GetBytes(message);
            }, Id);
        }
    }
}