namespace Lide.Core.Contract.Provider
{
    public interface ICompressionProvider
    {
        string CompressString(string text);
        string DecompressString(string compressedText);
    }
}