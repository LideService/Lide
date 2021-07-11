using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using Lide.TracingProxy.Reflection.Contract;
using Lide.TracingProxy.Reflection.Model;

namespace Lide.TracingProxy.Reflection
{
    public class FastMethodInfoProvider : IFastMethodInfoProvider
    {
        public MethodInfoDelegate CompileMethodInfo(MethodInfo methodInfo)
        {
            ParameterExpression instanceExpression = Expression.Parameter(typeof(object), "instance");
            ParameterExpression argumentsExpression = Expression.Parameter(typeof(object[]), "arguments");
            List<Expression> argumentExpressions = new ();
            ParameterInfo[] parameterInfos = methodInfo.GetParameters();
            UnaryExpression? instanceExpressionWithType = !methodInfo.IsStatic ? Expression.Convert(instanceExpression, methodInfo.ReflectedType!) : null;

            for (int idx = 0; idx < parameterInfos.Length; idx++)
            {
                ParameterInfo parameterInfo = parameterInfos[idx];
                argumentExpressions.Add(Expression.Convert(Expression.ArrayIndex(argumentsExpression, Expression.Constant(idx)), parameterInfo.ParameterType));
            }

            if (methodInfo.ReturnType == typeof(void))
            {
                MethodCallExpression callExpression = Expression.Call(instanceExpressionWithType, methodInfo, argumentExpressions);
                Action<object, object[]> voidDelegate = Expression.Lambda<Action<object, object[]>>(callExpression, instanceExpression, argumentsExpression).Compile();
                return WrapWithVoid(voidDelegate);
            }
            else
            {
                UnaryExpression callExpression = Expression.Convert(Expression.Call(instanceExpressionWithType, methodInfo, argumentExpressions), typeof(object));
                MethodInfoDelegate objectDelegate = Expression.Lambda<MethodInfoDelegate>(callExpression, instanceExpression, argumentsExpression).Compile();
                return objectDelegate;
            }
        }

        private MethodInfoDelegate WrapWithVoid(Action<object, object[]> fastMethodInfo)
        {
            return (instance, arguments) =>
            {
                fastMethodInfo(instance, arguments);
                return VoidReturn.Default;
            };
        }
    }
}