using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using Lide.Core.Contract.Facade;
using Lide.Core.Contract.Provider;
using Lide.Core.Model;

namespace Lide.Core.Provider
{
    public class MethodParamsSerializer : IParametersSerializer
    {
        private readonly ISerializerFacade _serializerFacade;
        private readonly ConcurrentDictionary<string, Type> _typesCache;

        public MethodParamsSerializer(ISerializerFacade serializerFacade)
        {
            _serializerFacade = serializerFacade;
            _typesCache = new ConcurrentDictionary<string, Type>();
        }

        public byte[] Serialize(object[] methodParams)
        {
            var result = methodParams.Select(param =>
            {
                var type = param.GetType();
                var typeName = param.GetType().FullName;
                _typesCache.TryAdd(typeName, type);

                return new SerializedParameter
                {
                    TypeName = typeName,
                    Data = _serializerFacade.Serialize(param),
                };
            }).ToList();

            return _serializerFacade.Serialize(result);
        }

        public object[] Deserialize(byte[] serialized)
        {
            var serializedParams = _serializerFacade.Deserialize<List<SerializedParameter>>(serialized);
            var result = serializedParams.Select(param =>
            {
                var type = _typesCache.GetOrAdd(param.TypeName, Type.GetType);
                var data = _serializerFacade.Deserialize(param.Data, type);
                return data;
            }).ToArray();

            return result;
        }

        public byte[] SerializeSingle(object methodParams)
        {
            var type = methodParams.GetType();
            var typeName = methodParams.GetType().FullName;
            _typesCache.TryAdd(typeName, type);

            return _serializerFacade.Serialize(new SerializedParameter
            {
                TypeName = typeName,
                Data = _serializerFacade.Serialize(methodParams),
            });
        }

        public object DeserializeSingle(byte[] serialized)
        {
            var serializedParam = _serializerFacade.Deserialize<SerializedParameter>(serialized);
            var type = _typesCache.GetOrAdd(serializedParam.TypeName, Type.GetType);
            var data = _serializerFacade.Deserialize(serializedParam.Data, type);
            return data;
        }
    }
}
