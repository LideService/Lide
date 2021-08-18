using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using System.Reflection.Metadata;
using System.Text;
using Lide.Core.Contract.Facade;
using Lide.Core.Contract.Provider;
using Lide.TracingProxy.Contract;
using Lide.TracingProxy.Model;

namespace Lide.Decorators
{
    public class DiagnosticsDecorator : IObjectDecorator
    {
        private readonly IFileWriter _fileWriter;
        private readonly ISignatureProvider _signatureProvider;
        private readonly IScopeProvider _scopeProvider;
        private static readonly Stopwatch Stopwatch = Stopwatch.StartNew();
        private readonly ConcurrentDictionary<int, long> _executionTimes;

        public DiagnosticsDecorator(
            IFileWriter fileWriter, 
            ISignatureProvider signatureProvider,
            IScopeProvider scopeProvider)
        {
            _fileWriter = fileWriter;
            _signatureProvider = signatureProvider;
            _scopeProvider = scopeProvider;
            _executionTimes = new ConcurrentDictionary<int, long>();
        }
        
        public string Id { get; } = "Lide.Diagnostics";
        public bool IsVolatile { get; } = false;
        
        public object[] ExecuteBeforeInvoke(object plainObject, MethodInfo methodInfo, object[] originalParameters, object[] editedParameters)
        {
            var methodHash = methodInfo.GetHashCode();
            var parametersHash = originalParameters.GetHashCode();
            var signatureHash = HashCode.Combine(methodHash, parametersHash);
            _executionTimes[signatureHash] = Stopwatch.ElapsedTicks;
            return originalParameters;
        }

        public ExceptionOrResult ExecuteAfterResult(object plainObject, MethodInfo methodInfo, object[] originalParameters, ExceptionOrResult originalEorR, ExceptionOrResult editedEorR)
        {
            var methodHash = methodInfo.GetHashCode();
            var parametersHash = originalParameters.GetHashCode();
            var signatureHash = HashCode.Combine(methodHash, parametersHash);
            long executionTime = -1;

            if (_executionTimes.ContainsKey(signatureHash))
            {
                _executionTimes.TryRemove(signatureHash, out var startTime);
                executionTime = Stopwatch.ElapsedTicks - startTime;
            }

            _fileWriter.AddToQueue(() =>
            {
                var scopeId = _scopeProvider.GetScopeId();
                var signature = _signatureProvider.GetMethodSignature(methodInfo);
                var message = $"[{scopeId}]: [{signature}] took {executionTime} ticks";
                return Encoding.ASCII.GetBytes(message);
            }, Id);

            return originalEorR;
        }
    }
}