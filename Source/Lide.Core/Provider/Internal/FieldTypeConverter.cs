using System;
using System.Linq;

namespace Lide.Core.Provider.Internal
{
    internal class FieldTypeConverter
    {
        public byte[] BuildFieldBytes(int hashCode, byte[] valueBytes)
        {
            var totalLength = valueBytes.Length + sizeof(int);
            var hashBytes = BitConverter.GetBytes(hashCode);
            var outputBytes = new byte[totalLength];
            hashBytes.CopyTo(outputBytes, 0);
            valueBytes.CopyTo(outputBytes, sizeof(int));
            return outputBytes;
        }

        public int ExtractHashCode(byte[] data)
        {
            var hashBytes = data.Take(sizeof(int)).ToArray();
            return BitConverter.ToInt32(hashBytes);
        }

        public byte[] ExtractValueBytes(byte[] data)
        {
            return data.Skip(sizeof(int)).ToArray();
        }
    }
}