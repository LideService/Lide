using System;
using System.Reflection;

namespace Lide.TracingProxy.Model
{
    public class MethodMetadata
    {
        private readonly object[] _originalParameters;
        private readonly Exception _originalException;
        private readonly object _originalResult;

        private object[] _editedParameters;
        private Exception _editedException;
        private object _editedResult;

        private bool _areParametersEdited;
        private bool _isExceptionEdited;
        private bool _isResultEdited;
        private bool _shouldReturn;
        private bool _shouldThrow;
        private bool _skipExecute;

        public MethodMetadata(
            object plainObject,
            MethodInfo methodInfo,
            object[] originalParameters,
            Exception exception,
            object result)
        {
            _originalParameters = originalParameters;
            _originalException = exception;
            _originalResult = result;
            PlainObject = plainObject;
            MethodInfo = methodInfo;
        }

        public object PlainObject { get; }
        public MethodInfo MethodInfo { get; }

        public object[] GetOriginalParameters() => _originalParameters;
        public object[] GetEditedParameters() => _editedParameters;
        public bool AreParametersEdited() => _areParametersEdited;
        public void SetParameters(object[] editedParameters)
        {
            _editedParameters = editedParameters;
            _areParametersEdited = true;
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
    }
}