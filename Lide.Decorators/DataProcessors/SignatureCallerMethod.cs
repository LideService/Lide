using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Lide.Decorators.Contract;

namespace Lide.Decorators.DataProcessors
{
    public class SignatureCallerMethod  : ISignatureCallerMethod
    {
        // Frames skipped:
        // 1. ExtractCallerInformation
        // 2. Proxy invocation
        private const int InitialFramesToSkip = 2;
        private static readonly List<string> ExcludeNamespaces = new () { "System", "Microsoft" };

        public string GetCallerSignature()
        {
            var skipFirst = false;
            StackTrace stackTrace = new (true);
            for (var i = InitialFramesToSkip; i < stackTrace.FrameCount; i++)
            {
                var stackFrame = stackTrace.GetFrame(i);
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

                return $"{frameFileName}-{frameMethodInfo.Name}:{frameLineNumber}";
            }

            return string.Empty;
        }
    }
}