using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using System.Runtime.ExceptionServices;
using System.Threading.Tasks;
using Lide.TracingProxy.Contract;
using Lide.TracingProxy.Model;

namespace Lide.TracingProxy.DecoratedProxy
{
    [SuppressMessage("Exception", "CA1031", Justification = "Never throws for any reason")]
    [SuppressMessage("ReSharper", "UnusedTypeParameter", Justification = "Partial class")]
    public partial class ProxyDecoratorTyped<TInterface>
        where TInterface : class
    {
        private MethodMetadataVolatile ExecuteBefore(MethodInfo methodInfo, object[] originalParameters)
        {
            var parametersMetadataVolatile = new ParametersMetadataVolatile(originalParameters);
            var returnMetadataVolatile = new ReturnMetadataVolatile(null, null);
            var metadataVolatile = new MethodMetadataVolatile(_originalObject, methodInfo, parametersMetadataVolatile, returnMetadataVolatile);
            ExecuteDecorators<IObjectDecoratorVolatile, MethodMetadataVolatile>(_volatileDecorators, decorator => decorator.ExecuteBeforeInvoke(metadataVolatile));

            var parametersMetadata = new ParametersMetadata(metadataVolatile.ParametersMetadataVolatile);
            var returnMetadata = new ReturnMetadata(metadataVolatile.ReturnMetadataVolatile);
            var metadata = new MethodMetadata(metadataVolatile, parametersMetadata, returnMetadata);
            ExecuteDecorators<IObjectDecoratorReadonly, MethodMetadata>(_readonlyDecorators, decorator => decorator.ExecuteBeforeInvoke(metadata));

            return metadataVolatile;
        }

        private MethodMetadataVolatile ExecuteMethodInfo(MethodMetadataVolatile executeBeforeMetadata)
        {
            ShouldThrow(executeBeforeMetadata);
            if (executeBeforeMetadata.ReturnMetadataVolatile.ShouldReturn())
            {
                return executeBeforeMetadata;
            }

            try
            {
                var methodInfo = executeBeforeMetadata.MethodInfo;
                var editedParameters = executeBeforeMetadata.ParametersMetadataVolatile.GetEditedParameters();
                var executableMethodInfo = _methodInfoCache.GetOrAdd(_originalObjectType, methodInfo, () => _methodInfoProvider.GetMethodInfoCompiled(methodInfo));
                var result = executableMethodInfo.Invoke(_originalObject, editedParameters);

                var returnMetadataVolatile = new ReturnMetadataVolatile(null, result is VoidReturn ? null : result);
                var methodMetadataVolatile = new MethodMetadataVolatile(executeBeforeMetadata, returnMetadataVolatile);
                return methodMetadataVolatile;
            }
            catch (Exception exception)
            {
                var returnMetadataVolatile = new ReturnMetadataVolatile(exception, null);
                var methodMetadataVolatile = new MethodMetadataVolatile(executeBeforeMetadata, returnMetadataVolatile);
                return methodMetadataVolatile;
            }
        }

        private object ExecuteAfter(MethodMetadataVolatile invokeMetadata)
        {
            ExecuteDecorators<IObjectDecoratorVolatile, MethodMetadataVolatile>(_volatileDecorators, decorator => decorator.ExecuteAfterResult(invokeMetadata));

            var parametersMetadata = new ParametersMetadata(invokeMetadata.ParametersMetadataVolatile);
            var returnMetadata = new ReturnMetadata(invokeMetadata.ReturnMetadataVolatile);
            var metadata = new MethodMetadata(invokeMetadata, parametersMetadata, returnMetadata);
            ExecuteDecorators<IObjectDecoratorReadonly, MethodMetadata>(_readonlyDecorators, decorator => decorator.ExecuteAfterResult(metadata));

            ShouldThrow(invokeMetadata);
            return invokeMetadata.ReturnMetadataVolatile.GetEditedResult();
        }

        private Task ExecuteAfterAsync(MethodMetadataVolatile invokeMetadata)
        {
            var resultAsTask = invokeMetadata.ReturnMetadataVolatile.GetEditedResult() as Task;
            var returnTask = resultAsTask?.ContinueWith(task =>
            {
                var returnMetadataVolatile = new ReturnMetadataVolatile(task.Exception, null);
                var metadataVolatile = new MethodMetadataVolatile(invokeMetadata, returnMetadataVolatile);
                ExecuteDecorators<IObjectDecoratorVolatile, MethodMetadataVolatile>(_volatileDecorators, decorator => decorator.ExecuteAfterResult(metadataVolatile));

                var parametersMetadata = new ParametersMetadata(invokeMetadata.ParametersMetadataVolatile);
                var returnMetadata = new ReturnMetadata(invokeMetadata.ReturnMetadataVolatile);
                var metadata = new MethodMetadata(invokeMetadata, parametersMetadata, returnMetadata);
                ExecuteDecorators<IObjectDecoratorReadonly, MethodMetadata>(_readonlyDecorators, decorator => decorator.ExecuteAfterResult(metadata));

                ShouldThrow(invokeMetadata);
            });

            return returnTask;
        }

        private Task<TReturnType> ExecuteAfterAsyncT<TReturnType>(MethodMetadataVolatile invokeMetadata)
        {
            var resultAsTask = invokeMetadata.ReturnMetadataVolatile.GetEditedResult() as Task<TReturnType>;
            var returnTask = resultAsTask?.ContinueWith(task =>
            {
                var returnMetadataVolatile = new ReturnMetadataVolatile(task.Exception, task.Result);
                var metadataVolatile = new MethodMetadataVolatile(invokeMetadata, returnMetadataVolatile);
                ExecuteDecorators<IObjectDecoratorVolatile, MethodMetadataVolatile>(_volatileDecorators, decorator => decorator.ExecuteAfterResult(metadataVolatile));

                var parametersMetadata = new ParametersMetadata(invokeMetadata.ParametersMetadataVolatile);
                var returnMetadata = new ReturnMetadata(invokeMetadata.ReturnMetadataVolatile);
                var metadata = new MethodMetadata(invokeMetadata, parametersMetadata, returnMetadata);
                ExecuteDecorators<IObjectDecoratorReadonly, MethodMetadata>(_readonlyDecorators, decorator => decorator.ExecuteAfterResult(metadata));

                ShouldThrow(invokeMetadata);
                return (TReturnType)metadataVolatile.ReturnMetadataVolatile.GetEditedResult();
            });

            return returnTask;
        }

        private static void ShouldThrow(MethodMetadataVolatile metadataVolatile)
        {
            var editedException = metadataVolatile.ReturnMetadataVolatile.GetEditedException();
            var shouldThrow = metadataVolatile.ReturnMetadataVolatile.ShouldThrow();
            if (shouldThrow && editedException != null)
            {
                ExceptionDispatchInfo.Capture(editedException).Throw();
            }
        }

        private void ExecuteDecorators<TDecorator, TData>(List<TDecorator> decorators, Action<TDecorator> execute)
            where TDecorator : IObjectDecorator
        {
            foreach (var decorator in decorators)
            {
                try
                {
                    execute(decorator);
                }
                catch (Exception e)
                {
                    _logError?.Invoke($"[Lide] Decorator {decorator.Id} has encountered an error {e}");
                }
            }
        }
    }
}
