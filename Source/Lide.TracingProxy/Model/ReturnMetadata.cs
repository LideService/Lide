using System;

namespace Lide.TracingProxy.Model
{
    public class ReturnMetadata
    {
        private readonly Exception _originalException;
        private readonly object _originalResult;

        private readonly Exception _editedException;
        private readonly object _editedResult;

        public ReturnMetadata(ReturnMetadataVolatile returnMetadataVolatile)
        {
            _originalException = returnMetadataVolatile.GetOriginalException();
            _editedException = returnMetadataVolatile.GetEditedException();
            _originalResult = returnMetadataVolatile.GetOriginalResult();
            _editedResult = returnMetadataVolatile.GetEditedResult();
        }

        public Exception GetOriginalException() => _originalException;
        public Exception GetEditedException() => _editedException;

        public object GetOriginalResult() => _originalResult;
        public object GetEditedResult() => _editedResult;
    }
}