using System;

namespace Lide.TracingProxy.Model
{
    public class ReturnMetadataVolatile
    {
        private readonly Exception _originalException;
        private readonly object _originalResult;

        private Exception _editedException;
        private object _editedResult;

        private bool _isExceptionEdited;
        private bool _isResultEdited;
        private bool _shouldReturn;
        private bool _shouldThrow;
        private bool _skipExecute;

        public ReturnMetadataVolatile(Exception exception, object result)
        {
            _originalException = exception;
            _originalResult = result;
        }

        public Exception GetOriginalException() => _originalException;
        public Exception GetEditedException() => _isExceptionEdited ? _editedException : _originalException;
        public bool IsExceptionEdited() => _isExceptionEdited;
        public void SetException(Exception editedException)
        {
            _editedException = editedException;
            _isExceptionEdited = true;
        }

        public object GetOriginalResult() => _originalResult;
        public object GetEditedResult() => _isResultEdited ? _editedResult : _originalResult;
        public bool IsResultEdited() => _isResultEdited;
        public void SetResult(object editedResult)
        {
            _editedResult = editedResult;
            _isResultEdited = true;
        }

        public bool ShouldThrow() => _shouldThrow;
        public bool ShouldReturn() => _shouldReturn;
        public bool SkipExecute() => _skipExecute;
        public void SetToThrow()
        {
            _skipExecute = true;
            _shouldThrow = true;
        }

        public void SetToReturn()
        {
            _skipExecute = true;
            _shouldReturn = true;
        }

        public ReturnMetadataVolatile Clone()
        {
            return new ReturnMetadataVolatile(_originalException, _originalResult)
            {
                _editedException = _editedException,
                _editedResult = _editedResult,
                _isExceptionEdited = _isExceptionEdited,
                _isResultEdited = _isResultEdited,
                _shouldReturn = _shouldReturn,
                _shouldThrow = _shouldThrow,
                _skipExecute = _skipExecute,
            };
        }
    }
}