namespace Lide.Core.Contract.Provider
{
    public interface ICompressionProvider
    {
        byte[] Compress(byte[] data);
        byte[] Decompress(byte[] compressed);
    }
}