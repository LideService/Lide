using System;
using System.Collections.Generic;
using Lide.Core.Model.Settings;

namespace Lide.Core.Contract.Provider
{
    public interface ISettingsProvider
    {
        AppSettings AppSettings { get; }
        string PropagateSettingsString { get; }
        bool AllowVolatileDecorators { get; }
        bool AllowReadonlyDecorators { get; }

        void Initialize(AppSettings appSettings, string propagateSettings);
        List<string> GetDecorators(Type type);
    }
}