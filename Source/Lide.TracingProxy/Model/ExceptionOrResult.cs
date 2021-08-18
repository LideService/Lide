using System;

namespace Lide.TracingProxy.Model
{
    public class ExceptionOrResult
    {
        public ExceptionOrResult()
        {
        }

        public ExceptionOrResult(Exception exception, object result)
        {
            Exception = exception;
            Result = result;
        }

        public Exception Exception { get; set; }
        public object Result { get; set; }
    }
}