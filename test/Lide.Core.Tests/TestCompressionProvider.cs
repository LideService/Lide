using System.Text;
using Lide.Core.Provider;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Lide.Core.Tests
{
    [TestClass]
    public class TestCompressionProvider
    {
        [TestMethod]
        public void That_AfterCompressionAndDecompression_OriginalValueIsRestored()
        {
            var provider = new CompressionProvider();
            var data = "Some text to validate its restored to the original value after compress-decompress";
            var bytes = Encoding.UTF8.GetBytes(data);
            var compressed = provider.Compress(bytes);
            var decompressed = provider.Decompress(compressed);
            Assert.AreEqual(data, Encoding.UTF8.GetString(decompressed));
        }
    }
}