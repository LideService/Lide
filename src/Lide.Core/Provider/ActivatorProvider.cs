using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using Lide.Core.Contract.Provider;

namespace Lide.Core.Provider;

public class ActivatorProvider : IActivatorProvider
{
    private readonly BindingFlags _flags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static;

    public object CreateObject(Type targetType)
    {
        var constructors = targetType.GetConstructors(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
        var shortest = constructors.OrderBy(x => x.GetParameters().Length).FirstOrDefault();
        if ((shortest?.GetParameters().Length ?? 0) == 0)
        {
            return Activator.CreateInstance(targetType);
        }

        var parameters = shortest!.GetParameters();
        var input = new List<object>();

        // ReSharper disable once LoopCanBeConvertedToQuery for readability
        foreach (var parameter in parameters)
        {
            var value = parameter.ParameterType.IsValueType
                ? Activator.CreateInstance(parameter.ParameterType)
                : Convert.ChangeType(null, parameter.ParameterType);
            input.Add(value);
        }

        return shortest.Invoke(input.ToArray());
    }

    public bool DeepCopyIntoTarget(object source, ref object target)
    {
        if (source == null && target == null)
        {
            return false;
        }

        if (source == null)
        {
            target = null;
            return false;
        }

        target ??= CreateObject(source!.GetType());
        return DeepCopyIntoExistingObject(source, target);
    }

    public bool DeepCopyIntoExistingObject(object source, object target)
    {
        if (source == null || target == null)
        {
            return false;
        }

        if (source == null)
        {
            return false;
        }

        var sourceType = source.GetType();
        var targetType = target.GetType();
        var circularReferences = new Dictionary<object, object>(new IdentityEqualityComparer<object>());
        if (sourceType != targetType || sourceType.IsValueType || sourceType == typeof(string))
        {
            return false;
        }

        CopyReferenceFields(source, target, circularReferences);
        return true;
    }

    private void CopyReferenceFields(object source, object target, Dictionary<object, object> circularReferences)
    {
        var members = source.GetType().GetFields(_flags).ToList();
        foreach (var member in members)
        {
            if (IsValueType(member.FieldType))
            {
                CopyValueField(member, source, target);
            }
            else if (member.FieldType.IsArray)
            {
                CopyArrayField(member, source, target, circularReferences);
            }
            else
            {
                CopyReferenceField(member, source, target, circularReferences);
            }
        }
    }

    // ReSharper disable once MemberCanBeMadeStatic.Local for consistency
    private bool IsValueType(Type type)
    {
        return type.IsValueType
               || type == typeof(string)
               || type == typeof(decimal)
               || type == typeof(DateTime);
    }

    private void CopyReferenceField(FieldInfo member, object source, object target, Dictionary<object, object> circularReferences)
    {
        if (member is { IsInitOnly: true, IsStatic: true })
        {
            return;
        }

        var sourceValue = member.GetValue(source);
        if (sourceValue == null)
        {
            member.SetValue(target, null);
            return;
        }

        if (circularReferences.ContainsKey(sourceValue))
        {
            member.SetValue(target, circularReferences[sourceValue]);
            return;
        }

        var actualSourceType = sourceValue.GetType();
        var targetValue = member.GetValue(target);
        if (targetValue == null)
        {
            targetValue = CreateObject(actualSourceType);
            member.SetValue(target, targetValue);
        }

        circularReferences.Add(sourceValue, targetValue);
        CopyReferenceFields(sourceValue, targetValue, circularReferences);
    }

    // ReSharper disable once MemberCanBeMadeStatic.Local for consistency
    private void CopyValueField(FieldInfo member, object source, object target)
    {
        if (member is { IsInitOnly: true, IsStatic: true })
        {
            return;
        }

        var value = member.GetValue(source);
        member.SetValue(target, value);
    }

    private void CopyArrayField(FieldInfo member, object source, object target, Dictionary<object, object> circularReferences)
    {
        var elementType = member.FieldType.GetElementType();
        var isArrayValueType = IsValueType(elementType);
        var sourceArray = (IList)(Array)member.GetValue(source);

        if (sourceArray == null)
        {
            member.SetValue(target, null);
            return;
        }

        // Always reset array reference as the original reference cant be resized.
        var targetArray = (IList)Array.CreateInstance(elementType!, sourceArray.Count);
        member.SetValue(target, targetArray);
        if (isArrayValueType)
        {
            for (var index = 0; index < sourceArray.Count; index++)
            {
                var arrayValue = sourceArray[index];
                targetArray[index] = arrayValue;
            }
        }
        else
        {
            for (var index = 0; index < sourceArray.Count; index++)
            {
                var sourceValue = sourceArray[index];
                if (sourceValue == null)
                {
                    targetArray[index] = null;
                    continue;
                }

                var actualSourceType = sourceValue.GetType();
                var targetValue = targetArray[index];
                if (targetValue == null)
                {
                    targetValue = CreateObject(actualSourceType);
                    targetArray[index] = targetValue;
                }

                CopyReferenceFields(sourceValue, targetValue, circularReferences);
            }
        }
    }

    private class IdentityEqualityComparer<T> : IEqualityComparer<T>
        where T : class
    {
        public int GetHashCode(T value)
        {
            return RuntimeHelpers.GetHashCode(value);
        }

        public bool Equals(T left, T right)
        {
            return ReferenceEquals(left, right);
        }
    }
}