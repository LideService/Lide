using Lide.Core.Contract.Provider;

namespace Lide.Core.Provider.Dummy
{
    public class DummyCompressionProvider : ICompressionProvider
    {
        public byte[] Compress(byte[] data)
        {
            return data;
        }

        public byte[] Decompress(byte[] compressed)
        {
            return compressed;
        }
    }
}