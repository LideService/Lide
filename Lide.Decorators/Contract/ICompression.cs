namespace Lide.Decorators.Contract
{
    public interface ICompression
    {
        string CompressString(string text);
        string DecompressString(string compressedText);
    }
}