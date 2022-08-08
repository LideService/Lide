using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Lide.Core.Contract.Provider;
using Lide.Core.Model;
using Lide.Core.Model.Settings;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Lide.WebApi.Controllers
{
    public class ExecuteLideController : Controller
    {
        private readonly HttpClient _httpClient;
        private readonly ICompressionProvider _compressionProvider;
        private readonly IBinarySerializeProvider _binarySerializeProvider;

        public ExecuteLideController(
            HttpClient httpClient,
            ICompressionProvider compressionProvider,
            IBinarySerializeProvider binarySerializeProvider)
        {
            _httpClient = httpClient;
            _compressionProvider = compressionProvider;
            _binarySerializeProvider = binarySerializeProvider;
        }

        [HttpPost]
        [Route("lide/replay")]
        public async Task ReplayLide(IFormFile formFile)
        {
            #pragma warning disable CA2007
            await using var lideResponseData = new MemoryStream();
            #pragma warning restore CA2007

            await formFile.CopyToAsync(lideResponseData).ConfigureAwait(false);
            var uploadedData = lideResponseData.ToArray();
            var uncompressed = _compressionProvider.Decompress(uploadedData);
            var lideResponse = _binarySerializeProvider.Deserialize<LideResponse>(uncompressed);

            var binaryContent = new ByteArrayContent(lideResponse.ContentData);
            binaryContent.Headers.Add(PropagateProperties.Depth, "1");
            binaryContent.Headers.Add(PropagateProperties.Enabled, "true");
            binaryContent.Headers.Add(PropagateProperties.PropagateSettings, HttpContext.Request.Headers[PropagateProperties.PropagateSettings].FirstOrDefault());
            await _httpClient.PostAsync(lideResponse.Path, binaryContent).ConfigureAwait(false);
        }
    }
}