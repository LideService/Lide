using System;
using System.Linq;
using System.Text;

namespace Lide.Core.Provider.Internal
{
    internal class FieldTypeConverter
    {
        public byte[] BuildFieldBytes(BytesMetadata bytesMetadata)
        {
            var typeBytes = GetTypeBytes(bytesMetadata);
            var totalLength = sizeof(byte) + sizeof(int) + sizeof(int) + typeBytes.Length + bytesMetadata.ValueBytes.Length;
            var resultBytes = new byte[totalLength];
            var offset = 0;
            resultBytes[0] = bytesMetadata.MetadataType;
            offset += sizeof(byte);
            BitConverter.GetBytes(bytesMetadata.FieldHashCode).CopyTo(resultBytes, offset);
            offset += sizeof(int);
            BitConverter.GetBytes(typeBytes.Length).CopyTo(resultBytes, offset);
            offset += sizeof(int);
            typeBytes.CopyTo(resultBytes, offset);
            offset += typeBytes.Length;
            bytesMetadata.ValueBytes.CopyTo(resultBytes, offset);

            return resultBytes;
        }

        public BytesMetadata ExtractMetadata(byte[] data)
        {
            var offset = 0;
            var metadataType = data[0];
            offset += sizeof(byte);
            var hashCode = BitConverter.ToInt32(data, offset);
            offset += sizeof(int);
            var typeLength = BitConverter.ToInt32(data, offset);
            offset += sizeof(int);
            var type = typeLength == 0 ? null : Type.GetType(Encoding.UTF8.GetString(data, offset, typeLength));
            offset += typeLength;
            var valueBytes = offset == data.Length ? Array.Empty<byte>() : data.Skip(offset).ToArray();
            return new BytesMetadata(metadataType, hashCode, type, valueBytes);
        }

        public int GetHashCode(byte[] data)
        {
            return BitConverter.ToInt32(data, sizeof(byte));
        }

        private byte[] GetTypeBytes(BytesMetadata bytesMetadata)
        {
            if (bytesMetadata.ActualFieldType == null)
            {
                return Array.Empty<byte>();
            }

            var typeString = Type.GetType(bytesMetadata.ActualFieldType.FullName!) == bytesMetadata.ActualFieldType
                ? bytesMetadata.ActualFieldType.FullName
                : bytesMetadata.ActualFieldType.AssemblyQualifiedName;
            return Encoding.UTF8.GetBytes(typeString!);
        }
    }
}