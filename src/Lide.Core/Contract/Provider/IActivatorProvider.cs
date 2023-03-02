using System;

namespace Lide.Core.Contract.Provider;

public interface IActivatorProvider
{
    object CreateObject(Type targetType);
    bool DeepCopyIntoExistingObject(object source, object target);
}