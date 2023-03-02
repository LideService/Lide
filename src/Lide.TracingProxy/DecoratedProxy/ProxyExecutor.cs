using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using System.Runtime.ExceptionServices;
using System.Threading;
using System.Threading.Tasks;
using Lide.TracingProxy.Contract;
using Lide.TracingProxy.Model;

namespace Lide.TracingProxy.DecoratedProxy;

[SuppressMessage("Exception", "CA1031", Justification = "Never throws for any reason")]
[SuppressMessage("ReSharper", "UnusedTypeParameter", Justification = "Partial class")]
public partial class ProxyDecorator<TInterface>
    where TInterface : class
{
    [DebuggerStepThrough]
    [DebuggerHidden]
    private MethodMetadataVolatile ExecuteBefore(MethodInfo methodInfo, object[] originalParameters)
    {
        var callId = Interlocked.Increment(ref CallCounter.CallId);
        var parametersMetadataVolatile = new ParametersMetadataVolatile(originalParameters);
        var returnMetadataVolatile = new ReturnMetadataVolatile(null, null);
        var metadataVolatile = new MethodMetadataVolatile(_originalObject, _isSingleton, methodInfo, parametersMetadataVolatile, returnMetadataVolatile, callId);
        ExecuteDecorators<IObjectDecoratorVolatile, MethodMetadataVolatile>(_volatileDecorators, decorator => decorator.ExecuteBeforeInvoke(metadataVolatile));

        var parametersMetadata = new ParametersMetadata(metadataVolatile.ParametersMetadataVolatile);
        var returnMetadata = new ReturnMetadata(metadataVolatile.ReturnMetadataVolatile);
        var metadata = new MethodMetadata(metadataVolatile, parametersMetadata, returnMetadata);
        ExecuteDecorators<IObjectDecoratorReadonly, MethodMetadata>(_readonlyDecorators, decorator => decorator.ExecuteBeforeInvoke(metadata));

        return metadataVolatile;
    }

    [DebuggerStepThrough]
    [DebuggerHidden]
    private MethodMetadataVolatile ExecuteMethodInfo(MethodMetadataVolatile executeBeforeMetadata)
    {
        ShouldThrow(executeBeforeMetadata);
        if (executeBeforeMetadata.ReturnMetadataVolatile.IsResultEdited())
        {
            var editedType = executeBeforeMetadata.ReturnMetadataVolatile.GetEditedResult().GetType();
            var returnType = executeBeforeMetadata.MethodInfo.ReturnType;
            if (editedType == returnType || editedType.IsSubclassOf(returnType))
            {
                return executeBeforeMetadata;
            }
        }

        try
        {
            var methodInfo = executeBeforeMetadata.MethodInfo;
            var editedParameters = executeBeforeMetadata.ParametersMetadataVolatile.GetEditedParameters();
            var result = methodInfo.Invoke(_originalObject, editedParameters);

            var returnMetadataVolatile = new ReturnMetadataVolatile(null, result);
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

    [DebuggerStepThrough]
    [DebuggerHidden]
    private object ExecuteAfter(MethodMetadataVolatile invokeMetadata)
    {
        ExecuteDecorators<IObjectDecoratorVolatile, MethodMetadataVolatile>(_volatileDecorators, decorator => decorator.ExecuteAfterResult(invokeMetadata));

        var parametersMetadata = new ParametersMetadata(invokeMetadata.ParametersMetadataVolatile);
        var returnMetadata = new ReturnMetadata(invokeMetadata.ReturnMetadataVolatile);
        var metadata = new MethodMetadata(invokeMetadata, parametersMetadata, returnMetadata);
        ExecuteDecorators<IObjectDecoratorReadonly, MethodMetadata>(_readonlyDecorators, decorator => decorator.ExecuteAfterResult(metadata));

        RepopulateOriginalParameters(invokeMetadata);
        ShouldThrow(invokeMetadata);
        var editedType = invokeMetadata.ReturnMetadataVolatile.GetEditedResult()?.GetType();
        var returnType = invokeMetadata.MethodInfo.ReturnType;
        return editedType == returnType || (editedType?.IsSubclassOf(returnType) ?? false)
            ? invokeMetadata.ReturnMetadataVolatile.GetEditedResult()
            : invokeMetadata.ReturnMetadataVolatile.GetOriginalResult();
    }

    [DebuggerStepThrough]
    [DebuggerHidden]
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

            RepopulateOriginalParameters(invokeMetadata);
            ShouldThrow(invokeMetadata);
        });

        return returnTask;
    }

    [DebuggerStepThrough]
    [DebuggerHidden]
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

            RepopulateOriginalParameters(invokeMetadata);
            ShouldThrow(invokeMetadata);
            return (TReturnType)metadataVolatile.ReturnMetadataVolatile.GetEditedResult();
        });

        return returnTask;
    }

    [DebuggerStepThrough]
    [DebuggerHidden]
    private void RepopulateOriginalParameters(MethodMetadataVolatile invokeMetadata)
    {
        if (!invokeMetadata.ParametersMetadataVolatile.AreParametersEdited())
        {
            return;
        }

        var originalParameters = invokeMetadata.ParametersMetadataVolatile.GetOriginalParameters();
        var editedParameters = invokeMetadata.ParametersMetadataVolatile.GetEditedParameters();
        var length = originalParameters.Length;
        for (var i = 0; i < length; i++)
        {
            _activatorProvider.DeepCopyIntoExistingObject(editedParameters[i], originalParameters[i]);
        }
    }

    [DebuggerStepThrough]
    [DebuggerHidden]
    private static void ShouldThrow(MethodMetadataVolatile metadataVolatile)
    {
        var editedException = metadataVolatile.ReturnMetadataVolatile.GetEditedException();
        var isResultEdited = metadataVolatile.ReturnMetadataVolatile.IsResultEdited();

        if (!isResultEdited && editedException != null)
        {
            ExceptionDispatchInfo.Capture(editedException.InnerException ?? editedException).Throw();
        }
    }

    [DebuggerStepThrough]
    [DebuggerHidden]
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