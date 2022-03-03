using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Lide.Core.Provider.Internal
{
    internal class ValueTypeConverter
    {
        private readonly Dictionary<Type, Func<object, byte[]>> _cachedGetBytes = new ();
        private readonly Dictionary<Type, Func<byte[], object>> _cachedGetObject = new ();

        public ValueTypeConverter()
        {
            GenerateGetBytes();
            GenerateGetObject();
        }

        // ReSharper disable once MemberCanBeMadeStatic.Global for consistency
        public bool IsValueType(Type type)
        {
            return (type.IsPrimitive && type.IsValueType) || type == typeof(string) || type == typeof(decimal) || type == typeof(DateTime);
        }

        public byte[] GetBytes(Type type, object data)
        {
            return _cachedGetBytes[type](data);
        }

        public object GetObject(Type type, byte[] data)
        {
            return _cachedGetObject[type](data);
        }

        private void GenerateGetObject()
        {
            _cachedGetObject.Add(typeof(byte), (data) => data[0]);
            _cachedGetObject.Add(typeof(string), Encoding.UTF8.GetString);
            _cachedGetObject.Add(typeof(decimal), (data) => ToDecimal(data));
            _cachedGetObject.Add(typeof(DateTime), (data) => DateTime.FromBinary(BitConverter.ToInt64(data)));

            var convertors = typeof(BitConverter).GetMethods()
                .Where(x => x.Name.StartsWith("To"))
                .Where(x => x.GetParameters().Length == 2)
                .Where(x => x.ReturnType != typeof(string))
                .Select(x => new
                {
                    Invoke = (Func<byte[], object>)(data => x.Invoke(null, new object[] { data, 0 })),
                    Type = x.ReturnType,
                }).ToList();

            foreach (var convertor in convertors)
            {
                _cachedGetObject.Add(convertor.Type, convertor.Invoke);
            }
        }

        private void GenerateGetBytes()
        {
            _cachedGetBytes.Add(typeof(byte), (data) => new byte[] { (byte)data });
            _cachedGetBytes.Add(typeof(string), (data) => data == null ? Array.Empty<byte>() : Encoding.UTF8.GetBytes((string)data));
            _cachedGetBytes.Add(typeof(decimal), (data) => GetBytes((decimal)data));
            _cachedGetBytes.Add(typeof(DateTime), (data) => BitConverter.GetBytes(((DateTime)data).ToBinary()));

            var convertors = typeof(BitConverter).GetMethods()
                .Where(x => x.Name == nameof(BitConverter.GetBytes))
                .Where(x => x.GetParameters().Length == 1)
                .Select(x => new
                {
                    Invoke = (Func<object, byte[]>)(data => (byte[])x.Invoke(null, new[] { data })),
                    Type = x.GetParameters().First().ParameterType,
                }).ToList();

            foreach (var convertor in convertors)
            {
                _cachedGetBytes.Add(convertor.Type, convertor.Invoke);
            }
        }

        private static decimal ToDecimal(byte[] bytes)
        {
            var bits = new int[4];
            bits[0] = ((bytes[0] | (bytes[1] << 8)) | (bytes[2] << 0x10)) | (bytes[3] << 0x18);
            bits[1] = ((bytes[4] | (bytes[5] << 8)) | (bytes[6] << 0x10)) | (bytes[7] << 0x18);
            bits[2] = ((bytes[8] | (bytes[9] << 8)) | (bytes[10] << 0x10)) | (bytes[11] << 0x18);
            bits[3] = ((bytes[12] | (bytes[13] << 8)) | (bytes[14] << 0x10)) | (bytes[15] << 0x18);

            return new decimal(bits);
        }

        private static byte[] GetBytes(decimal d)
        {
            var bytes = new byte[16];
            var bits = decimal.GetBits(d);
            var lo = bits[0];
            var mid = bits[1];
            var hi = bits[2];
            var flags = bits[3];

            bytes[0] = (byte)lo;
            bytes[1] = (byte)(lo >> 8);
            bytes[2] = (byte)(lo >> 0x10);
            bytes[3] = (byte)(lo >> 0x18);
            bytes[4] = (byte)mid;
            bytes[5] = (byte)(mid >> 8);
            bytes[6] = (byte)(mid >> 0x10);
            bytes[7] = (byte)(mid >> 0x18);
            bytes[8] = (byte)hi;
            bytes[9] = (byte)(hi >> 8);
            bytes[10] = (byte)(hi >> 0x10);
            bytes[11] = (byte)(hi >> 0x18);
            bytes[12] = (byte)flags;
            bytes[13] = (byte)(flags >> 8);
            bytes[14] = (byte)(flags >> 0x10);
            bytes[15] = (byte)(flags >> 0x18);

            return bytes;
        }
    }
}