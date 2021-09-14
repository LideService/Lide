using System;
using System.Collections;
using System.Linq;
using System.Reflection;

namespace Lide.Core.Provider.Internal
{
    internal class ObjectConverter
    {
        private readonly BindingFlags _flags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static;
        private readonly ValueTypeConverter _valueTypeConverters = new ();
        private readonly FieldTypeConverter _fieldTypeConverter = new ();
        private readonly BytesGlueProvider _bytesGlue = new ();
        private readonly ActivatorProvider _activatorProvider = new ();

        public byte[] Serialize(object data)
        {
            return ObjectReader(null, data, 0);
        }

        public object Deserialize(byte[] data)
        {
            return ObjectWriter(null, data);
        }

        private byte[] ObjectReader(Type fieldType, object data, int fieldHashCode)
        {
            if (data == null)
            {
                var metadata = BytesMetadata.GetNullMetadata(fieldHashCode);
                return _fieldTypeConverter.BuildFieldBytes(metadata);
            }

            var dataType = data.GetType();
            if (_valueTypeConverters.IsValueType(dataType))
            {
                var valueBytes = _valueTypeConverters.GetBytes(dataType, data);
                var metadata = dataType == fieldType
                    ? BytesMetadata.GetDefaultMetadata(fieldHashCode, valueBytes)
                    : BytesMetadata.GetMismatchMetadata(fieldHashCode, dataType, valueBytes);
                return _fieldTypeConverter.BuildFieldBytes(metadata);
            }

            if (dataType.IsArray)
            {
                var valueBytes = ArrayReader(dataType.GetElementType(), (Array)data);
                var metadata = dataType == fieldType
                    ? BytesMetadata.GetDefaultMetadata(fieldHashCode, valueBytes)
                    : BytesMetadata.GetMismatchMetadata(fieldHashCode, dataType, valueBytes);
                return _fieldTypeConverter.BuildFieldBytes(metadata);
            }

            var fields = dataType.GetFields(_flags)
                .Where(x => !(x.IsPrivate && x.IsStatic && x.IsInitOnly))
                .ToList();
            var fieldsData = new byte[fields.Count][];
            for (var fieldIndex = 0; fieldIndex < fields.Count; fieldIndex++)
            {
                var field = fields[fieldIndex];
                var value = field.GetValue(data);
                var valueBytes = ObjectReader(field.FieldType, value, field.GetHashCode());
                fieldsData[fieldIndex] = valueBytes;
            }

            var resultBytes = _bytesGlue.ConcatBytes(fieldsData);
            var resultMetadata = dataType == fieldType
                ? BytesMetadata.GetDefaultMetadata(fieldHashCode, resultBytes)
                : BytesMetadata.GetMismatchMetadata(fieldHashCode, dataType, resultBytes);
            return _fieldTypeConverter.BuildFieldBytes(resultMetadata);
        }

        private byte[] ArrayReader(Type elementType, IList arrayValues)
        {
            var arrayData = new byte[arrayValues.Count][];
            for (var index = 0; index < arrayValues.Count; index++)
            {
                var arrayValue = arrayValues[index];
                var arrayValueBytes = ObjectReader(elementType, arrayValue, 0);
                arrayData[index] = arrayValueBytes;
            }

            return _bytesGlue.ConcatBytes(arrayData);
        }

        private object ObjectWriter(Type fieldType, byte[] data)
        {
            if (data == null || data.Length == 0)
            {
                return null;
            }

            var metadata = _fieldTypeConverter.ExtractMetadata(data);
            if (metadata.IsNullValue)
            {
                return null;
            }

            var actualType = metadata.ActualFieldType ?? fieldType;
            if (_valueTypeConverters.IsValueType(actualType))
            {
                return _valueTypeConverters.GetObject(actualType, metadata.ValueBytes);
            }

            if (actualType.IsArray)
            {
                return ArrayWriter(actualType, metadata.ValueBytes);
            }

            var fields = actualType.GetFields(_flags).ToList();
            var target = _activatorProvider.CreateObject(actualType);
            var fieldsData = _bytesGlue.SplitBytes(metadata.ValueBytes);
            foreach (var fieldBytes in fieldsData)
            {
                var fieldHashCode = _fieldTypeConverter.GetHashCode(fieldBytes);
                var field = fields.First(x => x.GetHashCode() == fieldHashCode);
                var fieldValue = ObjectWriter(field.FieldType, fieldBytes);
                field.SetValue(target, fieldValue);
            }

            return target;
        }

        private Array ArrayWriter(Type arrayType, byte[] valueBytes)
        {
            var elementType = arrayType.GetElementType();
            var arrayData = _bytesGlue.SplitBytes(valueBytes);
            var array = Array.CreateInstance(elementType!, arrayData.Count);
            var arrayList = (IList)array;

            var index = 0;
            foreach (var arrayValueBytes in arrayData)
            {
                var arrayValue = ObjectWriter(elementType, arrayValueBytes);
                arrayList[index] = arrayValue;
                index++;
            }

            return array;
        }
    }
}