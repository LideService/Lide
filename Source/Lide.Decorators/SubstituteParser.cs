using System.Collections.Generic;
using System.IO;
using System.Linq;
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

        public SubstituteOwnContent SubstituteOwnRequest { get; private set; }
        public SubstituteOwnContent SubstituteOwnResponse { get; private set; }
        public List<SubstituteMethodBefore> BeforeMethods { get; } = new ();
        public List<SubstituteMethodAfter> AfterMethods { get; } = new ();
        public List<SubstituteOutgoingRequest> OutgoingRequests { get; } = new ();
        public List<SubstituteOutgoingResponse> OutgoingResponses { get; } = new ();

        public SubstituteMethodBefore GetBeforeMethod(string signature)
        {
            var before = BeforeMethods.FirstOrDefault(x => x.MethodSignature == signature);
            if (before != null)
            {
                BeforeMethods.Remove(before);
            }

            return before;
        }

        public SubstituteMethodAfter GetAfterMethod(long callId)
        {
            var after = AfterMethods.FirstOrDefault(x => x.CallId == callId);
            if (after != null)
            {
                AfterMethods.Remove(after);
            }

            return after;
        }

        public SubstituteOutgoingRequest GetOutgoingRequest(string path)
        {
            var request = OutgoingRequests.FirstOrDefault(x => x.Path == path);
            if (request != null)
            {
                OutgoingRequests.Remove(request);
            }

            return request;
        }

        public SubstituteOutgoingResponse GetOutgoingResponse(string path)
        {
            var response = OutgoingResponses.FirstOrDefault(x => x.Path == path);
            if (response != null)
            {
                OutgoingResponses.Remove(response);
            }

            return response;
        }

        public void LoadAll(Stream content)
        {
            var lastPosition = 0;
            while (lastPosition != -1)
            {
                lastPosition = LoadNext(content, lastPosition);
            }
        }

        private int LoadNext(Stream content, int lastPosition)
        {
            var binaryBatch = _streamBatchProvider.ReadNextBatch(content, lastPosition).Result;
            lastPosition = binaryBatch.EndPosition;
            if (lastPosition == -1)
            {
                return lastPosition;
            }

            var obj = _binarySerializeProvider.Deserialize(binaryBatch.Data);
            switch (obj)
            {
                case SubstituteMethodBefore substituteBeforeMethod:
                    BeforeMethods.Add(substituteBeforeMethod);
                    break;
                case SubstituteMethodAfter substituteAfterMethod:
                    AfterMethods.Add(substituteAfterMethod);
                    break;
                case SubstituteOutgoingRequest substituteOutgoingRequest:
                    OutgoingRequests.Add(substituteOutgoingRequest);
                    break;
                case SubstituteOutgoingResponse substituteOutgoingResponse:
                    OutgoingResponses.Add(substituteOutgoingResponse);
                    break;
                case SubstituteOwnContent substituteOwnContent:
                    SubstituteOwnRequest ??= substituteOwnContent;
                    SubstituteOwnResponse = substituteOwnContent;
                    break;
            }

            return lastPosition;
        }
    }
}