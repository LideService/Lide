using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using Lide.Core.Contract.Provider;
using Lide.Core.Model;

namespace Lide.Core.Provider
{
    public class MethodParamsSerializer : IParametersSerializer
    {
        private readonly ISerializeProvider _serializeProvider;
        private readonly ConcurrentDictionary<string, Type> _typesCache;

        public MethodParamsSerializer(ISerializeProvider serializeProvider)
        {
            _serializeProvider = serializeProvider;
            _typesCache = new ConcurrentDictionary<string, Type>();
        }

        public byte[] Serialize(object[] methodParams)
        {
            var result = methodParams.Select(param =>
            {
                if (param == null)
                {
                    return SerializedParameter.Null;
                }

                var type = param.GetType();
                var typeName = param.GetType().FullName;
                _typesCache.TryAdd(typeName, type);

                return new SerializedParameter
                {
                    TypeName = typeName,
                    Data = _serializeProvider.Serialize(param),
                };
            }).ToList();

            return _serializeProvider.Serialize(result);
        }

        public object[] Deserialize(byte[] serialized)
        {
            var serializedParams = _serializeProvider.Deserialize<List<SerializedParameter>>(serialized);
            var result = serializedParams.Select(param =>
            {
                if (param.TypeName == null || string.IsNullOrEmpty(param.TypeName))
                {
                    return null;
                }

                var type = _typesCache.GetOrAdd(param.TypeName, Type.GetType);
                var data = _serializeProvider.Deserialize(type, param.Data);
                return data;
            }).ToArray();

            return result;
        }

        public byte[] SerializeSingle(object methodParams)
        {
            if (methodParams == null)
            {
                return null;
            }

            var type = methodParams.GetType();
            var typeName = methodParams.GetType().FullName;
            _typesCache.TryAdd(typeName, type);

            return _serializeProvider.Serialize(new SerializedParameter
            {
                TypeName = typeName,
                Data = _serializeProvider.Serialize(methodParams),
            });
        }

        public object DeserializeSingle(byte[] serialized)
        {
            if (serialized == null || serialized.Length == 0)
            {
                return null;
            }

            var serializedParam = _serializeProvider.Deserialize<SerializedParameter>(serialized);
            var type = _typesCache.GetOrAdd(serializedParam.TypeName, Type.GetType);
            var data = _serializeProvider.Deserialize(type, serializedParam.Data);
            return data;
        }
    }
}
