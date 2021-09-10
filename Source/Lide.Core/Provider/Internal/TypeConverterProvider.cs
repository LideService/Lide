using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Lide.Core.Provider.Internal
{
    internal class TypeConverterProvider
    {
        private readonly BindingFlags _flags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance;
        private readonly ConcurrentDictionary<Type, Func<object, byte[]>> _cachedReads = new ();
        private readonly ConcurrentDictionary<Type, Func<byte[], object>> _cachedWrites = new ();
        private readonly ConcurrentDictionary<Type, List<Field>> _cachedFields = new ();
        private readonly ValueTypeConverter _valueTypeConverters = new ();
        private readonly FieldTypeConverter _fieldTypeConverter = new ();
        private readonly FieldsDataConverter _fieldsDataConverter = new ();
        private readonly ActivatorProvider _activatorProvider = new ();

        public Func<object, byte[]> GenerateReadAction(Type type)
        {
            if (_cachedReads.ContainsKey(type))
            {
                return _cachedReads[type];
            }

            if (_valueTypeConverters.IsValueType(type))
            {
                return _valueTypeConverters.GetBytes(type);
            }

            var fields = GetFields(type);
            byte[] ByteReader(object target)
            {
                if (target == null)
                {
                    return Array.Empty<byte>();
                }

                var fieldsData = new byte[fields.Count][];
                for (var fieldIndex = 0; fieldIndex < fields.Count; fieldIndex++)
                {
                    var field = fields[fieldIndex];
                    var fieldValue = field.GetValue(target);
                    var valueBytes = field.IsArray ? ArrayReader((Array)fieldValue, field) : field.Reader(fieldValue);
                    var fieldBytes = _fieldTypeConverter.BuildFieldBytes(field.HashCode, valueBytes);
                    fieldsData[fieldIndex] = fieldBytes;
                }

                return _fieldsDataConverter.ConcatFieldsData(fieldsData);
            }

            _cachedReads.TryAdd(type, ByteReader);
            return _cachedReads[type];
        }

        public Func<byte[], object> GenerateWriteAction(Type type)
        {
            if (_cachedWrites.ContainsKey(type))
            {
                return _cachedWrites[type];
            }

            if (_valueTypeConverters.IsValueType(type))
            {
                return _valueTypeConverters.GetObject(type);
            }

            var fields = GetFields(type);
            object ByteWriter(byte[] data)
            {
                var target = _activatorProvider.CreateObject(type);
                var fieldsData = _fieldsDataConverter.SplitFieldsData(data);
                foreach (var fieldBytes in fieldsData)
                {
                    var hashCode = _fieldTypeConverter.ExtractHashCode(fieldBytes);
                    var valueBytes = _fieldTypeConverter.ExtractValueBytes(fieldBytes);
                    var field = fields.First(x => x.HashCode == hashCode);
                    var fieldValue = field.IsArray ? ArrayWriter(valueBytes, field) : field.Writer(valueBytes);
                    field.SetValue(target, fieldValue);
                }

                return target;
            }

            _cachedWrites.TryAdd(type, ByteWriter);
            return _cachedWrites[type];
        }

        private Array ArrayWriter(byte[] valueBytes, Field field)
        {
            var arrayData = _fieldsDataConverter.SplitFieldsData(valueBytes);
            var arrayValues = Array.CreateInstance(field.ElementType, arrayData.Count);
            var arrayValuesAsList = (IList)arrayValues;
            var arrayIndex = 0;
            foreach (var arrayValueBytes in arrayData)
            {
                var arrayValue = field.ElementWriter(arrayValueBytes);
                arrayValuesAsList[arrayIndex++] = arrayValue;
            }

            return arrayValues;
        }

        private byte[] ArrayReader(Array arrayValues, Field field)
        {
            var arrayData = new byte[arrayValues.Length][];
            var arrayIndex = 0;
            foreach (var arrayValue in arrayValues)
            {
                var arrayValueBytes = field.ElementReader(arrayValue);
                arrayData[arrayIndex++] = arrayValueBytes;
            }

            return _fieldsDataConverter.ConcatFieldsData(arrayData);
        }

        private List<Field> GetFields(Type type)
        {
            var fields = type.GetFields(_flags)
                .Select(x => new Field
                {
                    IsArray = x.FieldType.IsArray,
                    HashCode = x.GetHashCode(),
                    GetValue = x.GetValue,
                    SetValue = x.SetValue,
                    Reader = x.FieldType.IsArray ? null : GenerateReadAction(x.FieldType),
                    Writer = x.FieldType.IsArray ? null : GenerateWriteAction(x.FieldType),
                    ElementType = x.FieldType.IsArray ? x.FieldType.GetElementType() : null,
                    ElementReader = x.FieldType.IsArray ? GenerateReadAction(x.FieldType.GetElementType()) : null,
                    ElementWriter = x.FieldType.IsArray ? GenerateWriteAction(x.FieldType.GetElementType()) : null,
                }).ToList();

            _cachedFields.TryAdd(type, fields);
            return _cachedFields[type];
        }

        private class Field
        {
            public int HashCode { get; init; }
            public bool IsArray { get; init; }
            public Type ElementType { get; init; }
            public Func<object, byte[]> ElementReader { get; init; }
            public Func<byte[], object> ElementWriter { get; init; }
            public Func<object, object> GetValue { get; init; }
            public Func<object, byte[]> Reader { get; init; }
            public Action<object, object> SetValue { get; init; }
            public Func<byte[], object> Writer { get; init; }
        }
    }
}