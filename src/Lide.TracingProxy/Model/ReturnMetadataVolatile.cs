using System;

namespace Lide.TracingProxy.Model;

public class ReturnMetadataVolatile
{
    private readonly Exception _originalException;
    private readonly object _originalResult;

    private Exception _editedException;
    private object _editedResult;

    private bool _isExceptionEdited;
    private bool _isResultEdited;

    public ReturnMetadataVolatile(Exception exception, object result)
    {
        _originalException = exception;
        _originalResult = result;
    }

    public Exception GetOriginalException() => _originalException;
    public Exception GetEditedException() => _isExceptionEdited ? _editedException : _originalException;
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

    public ReturnMetadataVolatile Clone()
    {
        return new ReturnMetadataVolatile(_originalException, _originalResult)
        {
            _editedException = _editedException,
            _editedResult = _editedResult,
            _isExceptionEdited = _isExceptionEdited,
            _isResultEdited = _isResultEdited,
        };
    }
}