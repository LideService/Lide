using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using Lide.Core.Contract.Facade;
using Lide.Core.Contract.Provider;
using Lide.Core.Model;

namespace Lide.Core.Provider
{
    public class MethodParamsSerializer : IMethodParamsSerializer
    {
        private readonly ISerializerFacade _serializerFacade;
        private readonly ConcurrentDictionary<string, Type> _typesCache;

        public MethodParamsSerializer(ISerializerFacade serializerFacade)
        {
            _serializerFacade = serializerFacade;
            _typesCache = new ConcurrentDictionary<string, Type>();
        }
        
        public string Serialize(object[] methodParams)
        {
            var serializedParams = new List<SerializedParameter>();
            var result = methodParams.Select(param =>
            {
                var type = param.GetType();
                var typeName = param.GetType().Name;
                _typesCache.TryAdd(typeName, type);

                return new SerializedParameter()
                {
                    TypeName = typeName,
                    Data = _serializerFacade.Serialize(param)
                };
            }).ToList();

            return _serializerFacade.Serialize(result);
        }

        public object[] Deserialize(string serialized)
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
    }
}
