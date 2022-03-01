using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Lide.Core.Contract.Facade;
using Lide.Core.Contract.Provider;
using Lide.Core.Model;

namespace Lide.Decorators
{
    public class SubstituteParser
    {
        private readonly IBinarySerializeProvider _binarySerializeProvider;
        private readonly IStreamBatchProvider _streamBatchProvider;

        public SubstituteParser(
            IBinarySerializeProvider binarySerializeProvider,
            IStreamBatchProvider streamBatchProvider)
        {
            _binarySerializeProvider = binarySerializeProvider;
            _streamBatchProvider = streamBatchProvider;
        }

        public SubstituteRequest SubstituteRequest { get; private set; }
        public List<SubstituteBefore> Befores { get; } = new ();
        public List<SubstituteAfter> Afters { get; } = new ();
        public List<SubstituteChild> Children { get; } = new ();

        public void Load(Stream content)
        {
            var lastPosition = 0;
            while (true)
            {
                var binaryBatch = _streamBatchProvider.ReadNextBatch(content, lastPosition).Result;
                lastPosition = binaryBatch.EndPosition;
                if (lastPosition == -1)
                {
                    break;
                }

                var obj = _binarySerializeProvider.Deserialize(binaryBatch.Data);
                switch (obj)
                {
                    case SubstituteRequest substituteRequest:
                        SubstituteRequest = substituteRequest;
                        break;
                    case SubstituteBefore substituteBefore:
                        Befores.Add(substituteBefore);
                        break;
                    case SubstituteAfter substituteAfter:
                        Afters.Add(substituteAfter);
                        break;
                    case SubstituteChild substituteChild:
                        Children.Add(substituteChild);
                        break;
                }
            }
        }
    }
}