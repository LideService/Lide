using System;
using System.Collections.Generic;
using System.Linq;
using Lide.Core.Contract.Facade;
using Lide.Core.Contract.Provider;
using Lide.Core.Model;

namespace Lide.Decorators.Substitute
{
    public class SubstituteLoader : ISubstituteLoader
    {
        private readonly IFileFacade _fileFacade;
        private readonly ICompressionProvider _compressionProvider;
        private readonly IBinarySerializeProvider _binarySerializeProvider;
        private readonly Dictionary<string, SubstituteBefore> _befores;
        private readonly Dictionary<long, SubstituteAfter> _afters;
        private readonly Dictionary<long, long> _ids;

        public SubstituteLoader(
            IFileFacade fileFacade,
            ICompressionProvider compressionProvider,
            IBinarySerializeProvider binarySerializeProvider)
        {
            _fileFacade = fileFacade;
            _compressionProvider = compressionProvider;
            _binarySerializeProvider = binarySerializeProvider;
            _befores = new Dictionary<string, SubstituteBefore>();
            _afters = new Dictionary<long, SubstituteAfter>();
            _ids = new Dictionary<long, long>();
        }

        internal List<SubstituteBefore> Befores => _befores.Values.ToList();
        internal List<SubstituteAfter> Afters => _afters.Values.ToList();

        public SubstituteBefore GetBefore(long callId, string methodSignature)
        {
            if (_befores.ContainsKey(methodSignature))
            {
                var before = _befores[methodSignature];
                _ids[callId] = before.CallId;
                return before;
            }

            return null;
        }

        public SubstituteAfter GetAfter(long callId)
        {
            if (_ids.ContainsKey(callId))
            {
                var executionCallId = _ids[callId];
                if (_afters.ContainsKey(executionCallId))
                {
                    return _afters[executionCallId];
                }
            }

            return null;
        }

        public void Load(string filePath)
        {
            var readPosition = 0;
            while (true)
            {
                var typesBatch = _fileFacade.ReadNextBatch(filePath, readPosition).Result;
                readPosition = typesBatch.EndPosition;
                if (typesBatch.EndPosition == -1)
                {
                    break;
                }

                var dataBatch = _fileFacade.ReadNextBatch(filePath, readPosition).Result;
                readPosition = dataBatch.EndPosition;
                var type = (SubstituteType)BitConverter.ToInt32(typesBatch.Data);
                var data = _compressionProvider.Decompress(dataBatch.Data);
                switch (type)
                {
                    case SubstituteType.After:
                        var result = _binarySerializeProvider.Deserialize<SubstituteAfter>(data);
                        _afters.TryAdd(result.CallId, result);
                        break;
                    case SubstituteType.Before:
                        var before = _binarySerializeProvider.Deserialize<SubstituteBefore>(data);
                        _befores.TryAdd(before.MethodSignature, before);
                        break;
                }
            }
        }
    }
}