using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using Lide.Core.Contract.Provider;
using Lide.Core.Model.Settings;
using Lide.TracingProxy.Contract;

namespace Lide.Decorators
{
    public class PropagateContentHandler : IPropagateContentHandler
    {
        private readonly List<IRequestResponseDecorator> _decorators;
        private readonly IBinarySerializeProvider _binarySerializeProvider;
        private readonly ISettingsProvider _settingsProvider;

        public PropagateContentHandler(
            IEnumerable<IObjectDecoratorReadonly> readonlyDecorators,
            IEnumerable<IObjectDecoratorVolatile> volatileDecorators,
            IBinarySerializeProvider binarySerializeProvider,
            ISettingsProvider settingsProvider)
        {
            _decorators = new List<IRequestResponseDecorator>();
            _binarySerializeProvider = binarySerializeProvider;
            _settingsProvider = settingsProvider;

            _decorators.AddRange(readonlyDecorators);
            _decorators.AddRange(volatileDecorators);
        }

        public void ParseDataFromOwnRequest(ConcurrentDictionary<string, byte[]> content)
        {
            _decorators.ForEach(x =>
            {
                if (_settingsProvider.IsDecoratorIncluded(x.Id))
                {
                    x.ParseOwnRequest(content);
                }
            });
        }

        public void PrepareDataForOutgoingRequest(ConcurrentDictionary<string, byte[]> container, string path, long requestId, byte[] content)
        {
            var settings = _settingsProvider.PropagateSettings;
            var serialized = _binarySerializeProvider.Serialize(settings);
            container.TryAdd(PropagateProperties.PropagateSettings, serialized);
            _decorators.ForEach(x =>
            {
                if (_settingsProvider.IsDecoratorIncluded(x.Id))
                {
                    x.PrepareOutgoingRequest(container, path, requestId, content);
                }
            });
        }

        public void ParseDataFromOutgoingResponse(ConcurrentDictionary<string, byte[]> content, string path, long requestId, Exception exception)
        {
            _decorators.ForEach(x =>
            {
                if (_settingsProvider.IsDecoratorIncluded(x.Id))
                {
                    x.ParseOutgoingResponse(content, path, requestId, exception);
                }
            });
        }

        public void PrepareDataForOwnResponse(ConcurrentDictionary<string, byte[]> container)
        {
            _decorators.ForEach(x =>
            {
                if (_settingsProvider.IsDecoratorIncluded(x.Id))
                {
                    x.PrepareOwnResponse(container);
                }
            });
        }
    }
}