using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Lide.TracingProxy.Contract;
using Lide.TracingProxy.Model;

namespace Lide.TracingProxy.Reflection
{
    public class ProviderMethodInfoCompiled : IMethodInfoProvider
    {
        public static readonly IMethodInfoProvider Singleton = new ProviderMethodInfoCompiled();

        public MethodInfoCompiled GetMethodInfoCompiled(MethodInfo methodInfo)
        {
            var instanceExpression = Expression.Parameter(typeof(object), "instance");
            var argumentsExpression = Expression.Parameter(typeof(object[]), "arguments");
            var parameterInfos = methodInfo.GetParameters();
            var instanceExpressionWithType = !methodInfo.IsStatic ? Expression.Convert(instanceExpression, methodInfo.ReflectedType!) : null;

            var argumentExpressions = parameterInfos
                .Select((parameterInfo, idx) => Expression.Convert(Expression.ArrayIndex(argumentsExpression, Expression.Constant(idx)), parameterInfo.ParameterType))
                .Cast<Expression>().ToList();

            if (methodInfo.ReturnType == typeof(void))
            {
                var callExpression = Expression.Call(instanceExpressionWithType, methodInfo, argumentExpressions);
                var voidDelegate = Expression
                    .Lambda<Action<object, object[]>>(callExpression, instanceExpression, argumentsExpression)
                    .Compile();
                return WrapWithVoid(voidDelegate);
            }
            else
            {
                var callExpression = Expression.Convert(Expression.Call(instanceExpressionWithType, methodInfo, argumentExpressions), typeof(object));
                var objectCompiled = Expression.Lambda<MethodInfoCompiled>(callExpression, instanceExpression, argumentsExpression).Compile();
                return objectCompiled;
            }
        }

        private static MethodInfoCompiled WrapWithVoid(Action<object, object[]> fastMethodInfo)
        {
            return (instance, arguments) =>
            {
                fastMethodInfo(instance, arguments);
                return VoidReturn.Default;
            };
        }
    }
}