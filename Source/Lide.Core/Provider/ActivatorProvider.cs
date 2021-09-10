using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Lide.Core.Contract.Provider;

namespace Lide.Core.Provider
{
    public class ActivatorProvider : IActivatorProvider
    {
        private readonly Dictionary<Type, Action<object, object>> _cachedCloning = new ();
        private readonly BindingFlags _flags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance;

        public object CreateObject(Type targetType)
        {
            var constructors = targetType.GetConstructors(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            var shortest = constructors.OrderBy(x => x.GetParameters().Length).First();
            if (shortest.GetParameters().Length == 0)
            {
                return Activator.CreateInstance(targetType);
            }

            var parameters = shortest.GetParameters();
            var input = new List<object>();
            foreach (var parameter in parameters)
            {
                var value = parameter.ParameterType.IsValueType
                    ? Activator.CreateInstance(parameter.ParameterType)
                    : Convert.ChangeType(null, parameter.ParameterType);
                input.Add(value);
            }

            return shortest.Invoke(input.ToArray());
        }

        public bool DeepCopyIntoExistingObject(object source, object target)
        {
            if (source == null || target == null)
            {
                return false;
            }

            var sourceType = source.GetType();
            var targetType = target.GetType();
            if (sourceType != targetType || sourceType.IsValueType || sourceType == typeof(string))
            {
                return false;
            }

            if (!_cachedCloning.ContainsKey(sourceType))
            {
                GenerateCopyActions(sourceType);
            }

            _cachedCloning[sourceType](source, target);
            return true;
        }

        private Action<object, object> GenerateCopyActions(Type type)
        {
            if (_cachedCloning.ContainsKey(type))
            {
                return _cachedCloning[type];
            }

            var actions = new List<Action<object, object>>();
            var members = type.GetFields(_flags).ToList();
            for (var index = 0; index < members.Count; index++)
            {
                var member = members[index];
                if (member.FieldType.IsValueType || member.FieldType == typeof(string))
                {
                    actions.Add((source, target) =>
                    {
                        var value = member.GetValue(source);
                        member.SetValue(target, value);
                    });

                    continue;
                }

                actions.Add((source, target) =>
                {
                    var sourceValue = member.GetValue(source);
                    var targetValue = member.GetValue(target);
                    if (sourceValue == null)
                    {
                        member.SetValue(target, null);
                        return;
                    }

                    if (targetValue == null)
                    {
                        targetValue = CreateObject(member.FieldType);
                        member.SetValue(target, targetValue);
                    }

                    GenerateCopyActions(member.FieldType)(sourceValue, targetValue);
                });
            }

            void TypeCopier(object source, object target)
            {
                actions.ForEach(x => x(source, target));
            }

            _cachedCloning.TryAdd(type, TypeCopier);
            return _cachedCloning[type];
        }
    }
}