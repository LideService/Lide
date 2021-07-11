using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Lide.TracingProxy.DataProcessors.Contract;
using Lide.TracingProxy.DataProcessors.Model;

namespace Lide.TracingProxy.DataProcessors
{
    public class StackTraceExtractor : IStackTraceExtractor
    {
        // Frames skipped:
        // 1. ExtractCallerInformation
        // 2. Proxy invocation
        private static readonly int InitialFramesToSkip = 2;
        private static readonly List<string> ExcludeNamespaces = new() { "System", "Microsoft" };

        public CallerInformation ExtractCallerInformation()
        {
            bool skipFirst = false;
            StackTrace stackTrace = new(true);
            for (int i = InitialFramesToSkip; i < stackTrace.FrameCount; i++)
            {
                StackFrame? stackFrame = stackTrace.GetFrame(i);
                if (stackFrame == null)
                {
                    continue;
                }

                var frameMethodInfo = stackFrame.GetMethod();
                var frameFileName = stackFrame.GetFileName();
                var frameLineNumber = stackFrame.GetFileLineNumber();
                var stackFrameFullMethodName = frameMethodInfo?.DeclaringType?.FullName;
                if (frameMethodInfo == null
                    || stackFrameFullMethodName == null
                    || ExcludeNamespaces.Any(x => stackFrameFullMethodName.StartsWith(x))
                    || string.IsNullOrEmpty(frameFileName)
                    || frameLineNumber == 0)
                {
                    continue;
                }

                if (!skipFirst)
                {
                    skipFirst = true;
                    continue;
                }

                return new CallerInformation
                {
                    CallerFileName = frameFileName,
                    CallerMethodName = frameMethodInfo.Name,
                    CallerLineNumber = frameLineNumber,
                };
            }
            
            return new CallerInformation
            {
                CallerFileName = string.Empty,
                CallerMethodName = string.Empty,
                CallerLineNumber = 0,
            };
        }
    }
}