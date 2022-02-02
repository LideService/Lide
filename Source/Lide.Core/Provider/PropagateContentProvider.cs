using System.Collections.Generic;
using System.Threading.Tasks;
using Lide.Core.Contract.Facade;
using Lide.Core.Contract.Provider;

namespace Lide.Core.Provider
{
    public class PropagateContentProvider : IPropagateContentProvider, IPropagateContentController
    {
        private readonly IFileFacade _fileFacade;
        private readonly IPathFacade _pathFacade;
        private readonly ICleanupService _cleanupService;
        private readonly IBinarySerializeProvider _binarySerializeProvider;

        private readonly Dictionary<string, string> _filesForRequest = new ();
        private readonly Dictionary<string, byte[]> _datasForRequest = new ();
        private readonly Dictionary<string, string> _filesFromResponse = new ();
        private readonly Dictionary<string, byte[]> _datasFromResponse = new ();

        private readonly Dictionary<string, string> _filesForResponse = new ();
        private readonly Dictionary<string, byte[]> _datasForResponse = new ();
        private readonly Dictionary<string, string> _filesFromRequest = new ();
        private readonly Dictionary<string, byte[]> _datasFromRequest = new ();

        public PropagateContentProvider(
            IFileFacade fileFacade,
            IPathFacade pathFacade,
            ICleanupService cleanupService,
            IBinarySerializeProvider binarySerializeProvider)
        {
            _fileFacade = fileFacade;
            _pathFacade = pathFacade;
            _cleanupService = cleanupService;
            _binarySerializeProvider = binarySerializeProvider;
        }

        public void PutFileForRequest(string name, string filePath) => _filesForRequest.Add(name, filePath);
        public void PutDataForRequest(string name, byte[] data) => _datasForRequest.Add(name, data);
        public string ReadFileFromResponse(string name) => _filesFromResponse[name];
        public byte[] ReadDataFromResponse(string name) => _datasFromResponse[name];

        public void PutFileForResponse(string name, string filePath) => _filesForResponse.Add(name, filePath);
        public void PutDataForResponse(string name, byte[] data) => _datasForResponse.Add(name, data);
        public string ReadFileFromRequest(string name) => _filesFromRequest[name];
        public byte[] ReadDataFromRequest(string name) => _datasFromRequest[name];

        public async Task<byte[]> GetDataForRequest()
        {
            var propagateData = new PropagateData();
            foreach (var (key, value) in _datasForRequest)
            {
                propagateData.DataBytes.Add(key, value);
            }

            foreach (var (key, value) in _filesForRequest)
            {
                var data = await _fileFacade.ReadWholeFle(value).ConfigureAwait(false);
                propagateData.FileBytes.Add(key, data);
            }

            return _binarySerializeProvider.Serialize(propagateData);
        }

        public async Task PutDataFromRequest(byte[] data)
        {
            var propagateData = _binarySerializeProvider.Deserialize<PropagateData>(data);
            foreach (var (key, bytes) in propagateData.DataBytes)
            {
                _datasFromRequest.Add(key, bytes);
            }

            foreach (var (key, bytes) in propagateData.FileBytes)
            {
                var filePath = _pathFacade.Combine(_pathFacade.GetTempPath(), _fileFacade.GetFileName());
                await _fileFacade.WriteWholeFile(filePath, bytes).ConfigureAwait(false);
                _cleanupService.RegisterCleanupFile(filePath);
                _filesFromRequest.Add(key, filePath);
            }
        }

        public async Task<byte[]> GetDataForResponse()
        {
            var propagateData = new PropagateData();
            foreach (var (key, value) in _datasForResponse)
            {
                propagateData.DataBytes.Add(key, value);
            }

            foreach (var (key, value) in _filesForResponse)
            {
                var data = await _fileFacade.ReadWholeFle(value).ConfigureAwait(false);
                propagateData.FileBytes.Add(key, data);
            }

            return _binarySerializeProvider.Serialize(propagateData);
        }

        public async Task PutDataFromResponse(byte[] data)
        {
            var propagateData = _binarySerializeProvider.Deserialize<PropagateData>(data);
            foreach (var (key, bytes) in propagateData.DataBytes)
            {
                _datasFromResponse.Add(key, bytes);
            }

            foreach (var (key, bytes) in propagateData.FileBytes)
            {
                var filePath = _pathFacade.Combine(_pathFacade.GetTempPath(), _fileFacade.GetFileName());
                await _fileFacade.WriteWholeFile(filePath, bytes).ConfigureAwait(false);
                _cleanupService.RegisterCleanupFile(filePath);
                _filesFromResponse.Add(key, filePath);
            }
        }

        private class PropagateData
        {
            public readonly Dictionary<string, byte[]> DataBytes = new ();
            public readonly Dictionary<string, byte[]> FileBytes = new ();
        }
    }
}