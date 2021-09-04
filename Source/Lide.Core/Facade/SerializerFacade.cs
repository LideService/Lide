using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Lide.Core.Contract.Facade;
using Newtonsoft.Json;
using Newtonsoft.Json.Bson;
using Newtonsoft.Json.Serialization;

namespace Lide.Core.Facade
{
    public class SerializerFacade : ISerializerFacade
    {
        private static readonly JsonSerializerSettings Settings = new ()
        {
            ContractResolver = new MyContractResolver(),
            NullValueHandling = NullValueHandling.Include,
        };
        private static readonly JsonSerializer FullSerializer = JsonSerializer.Create(Settings);

        public byte[] Serialize<T>(T data)
        {
            using var ms = new MemoryStream();
            using var writer = new BsonDataWriter(ms);
            FullSerializer.Serialize(writer, data);

            return ms.ToArray();
        }

        public string SerializeToString<T>(T data)
        {
            return JsonConvert.SerializeObject(data);
        }

        public T Deserialize<T>(byte[] data)
        {
            using var ms = new MemoryStream(data);
            using var reader = new BsonDataReader(ms);
            return FullSerializer.Deserialize<T>(reader);
        }

        public object Deserialize(byte[] data, Type type)
        {
            using var ms = new MemoryStream(data);
            using var reader = new BsonDataReader(ms);
            return FullSerializer.Deserialize(reader, type);
        }

        public void PopulateObject(byte[] data, object target)
        {
            using var ms = new MemoryStream(data);
            using var reader = new BsonDataReader(ms);
            FullSerializer.Populate(reader, target);
        }

        public void PopulateObject(object source, object target)
        {
            PopulateObject(Serialize(source), target);
        }

        private class MyContractResolver : DefaultContractResolver
        {
            protected override IList<JsonProperty> CreateProperties(Type type, MemberSerialization memberSerialization)
            {
                var props = type.GetProperties(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)
                    .Select(p => CreateProperty(p, memberSerialization))
                    .Union(type.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)
                        .Select(f => CreateProperty(f, memberSerialization)))
                    .ToList();
                props.ForEach(p =>
                {
                    p.Writable = true;
                    p.Readable = true;
                });
                return props;
            }
        }
    }
}