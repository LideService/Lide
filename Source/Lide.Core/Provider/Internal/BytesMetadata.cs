using System;

namespace Lide.Core.Provider.Internal
{
    internal class BytesMetadata
    {
        public BytesMetadata(byte metadataType, int fieldHashCode, Type actualFieldType, byte[] valueBytes)
        {
            MetadataType = metadataType;
            FieldHashCode = fieldHashCode;
            ActualFieldType = actualFieldType;
            ValueBytes = valueBytes;
        }

        public byte MetadataType { get; }
        public int FieldHashCode { get; set; }
        public Type ActualFieldType { get; set; }
        public byte[] ValueBytes { get; set; }

        public bool IsDefault => MetadataType == 0;
        public bool IsNullValue => MetadataType == 1;
        public bool IsMismatch => MetadataType == 2;
        public bool IsRoot => MetadataType == 3;

        public static BytesMetadata GetDefaultMetadata(int fieldHashCode, byte[] valueBytes) => new BytesMetadata(0, fieldHashCode, null, valueBytes);
        public static BytesMetadata GetNullMetadata(int fieldHashCode) => new BytesMetadata(1, fieldHashCode, null, Array.Empty<byte>());
        public static BytesMetadata GetMismatchMetadata(int fieldHashCode, Type actualFieldType, byte[] valueBytes) => new BytesMetadata(2, fieldHashCode, actualFieldType, valueBytes);
        public static BytesMetadata GetRootMetadata(Type actualFieldType, byte[] valueBytes) => new BytesMetadata(3, 0, actualFieldType, valueBytes);
    }
}