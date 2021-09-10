using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

namespace Lide.Core.Provider.Internal
{
    internal class FieldsDataConverter
    {
        public byte[] ConcatFieldsData(IReadOnlyCollection<byte[]> fields)
        {
            var totalLength = fields.Sum(x => x.Length) + fields.Count * sizeof(int);
            var outputBytes = new byte[totalLength];

            var startIndex = 0;
            foreach (var fieldData in fields)
            {
                var sizeBytes = BitConverter.GetBytes(fieldData.Length);
                sizeBytes.CopyTo(outputBytes, startIndex);
                fieldData.CopyTo(outputBytes, startIndex + sizeof(int));
                startIndex += fieldData.Length + sizeof(int);
            }

            return outputBytes;
        }

        public IReadOnlyCollection<byte[]> SplitFieldsData(byte[] data)
        {
            var result = new List<byte[]>();

            var startIndex = 0;
            while (true)
            {
                if (startIndex >= data.Length)
                {
                    break;
                }

                var fieldSize = BitConverter.ToInt32(data, startIndex);
                var fieldData = data.Skip(startIndex + sizeof(int)).Take(fieldSize).ToArray();
                startIndex += sizeof(int) + fieldData.Length;
                result.Add(fieldData);
            }

            return result;
        }
    }
}