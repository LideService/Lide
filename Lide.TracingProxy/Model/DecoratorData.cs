using System;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using System.Threading.Tasks;

namespace Lide.TracingProxy.Model
{
    [SuppressMessage("Exception", "CA1819", Justification = "Data is not readonly")]
    public class DecoratorData
    {
        public object OriginalObject { get; set; }
        public MethodInfo MethodInfo { get; set; }
        public object[] MethodParameters { get; set; }
        public object Result { get; set; }
        public Exception Exception { get; set; }
        public AggregateException AggregateException { get; set; }
    }
}