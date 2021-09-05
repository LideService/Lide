using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using Lide.Core.Contract.Facade;
using Newtonsoft.Json;
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
        ////private static readonly JsonSerializer FullSerializer = JsonSerializer.Create(Settings);

        public byte[] Serialize<T>(T data)
        {
            var serialized = JsonConvert.SerializeObject(data, Settings);
            var bytes = Encoding.UTF8.GetBytes(serialized);
            return bytes;
            ////using var ms = new MemoryStream();
            ////using var writer = new BsonDataWriter(ms);
            ////FullSerializer.Serialize(writer, data);
            ////return ms.ToArray();
        }

        public string SerializeToString<T>(T data)
        {
            return JsonConvert.SerializeObject(data);
        }

        public T Deserialize<T>(byte[] data)
        {
            var serialized = Encoding.UTF8.GetString(data);
            var result = JsonConvert.DeserializeObject<T>(serialized, Settings);
            return result;
            ////using var ms = new MemoryStream(data);
            ////using var reader = new BsonDataReader(ms);
            ////return FullSerializer.Deserialize<T>(reader);
        }

        public object Deserialize(byte[] data, Type type)
        {
            var serialized = Encoding.UTF8.GetString(data);
            var result = JsonConvert.DeserializeObject(serialized, type, Settings);
            return result;
            ////using var ms = new MemoryStream(data);
            ////using var reader = new BsonDataReader(ms);
            ////return FullSerializer.Deserialize(reader, type);
        }

        public void PopulateObject(byte[] data, object target)
        {
            var serialized = Encoding.UTF8.GetString(data);
            JsonConvert.PopulateObject(serialized, target);
            ////using var ms = new MemoryStream(data);
            ////using var reader = new BsonDataReader(ms);
            ////FullSerializer.Populate(serialized, target);
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