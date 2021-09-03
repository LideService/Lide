using System;
using System.Collections.Generic;
using Lide.Core.Model.Settings;

namespace Lide.Core.Contract.Provider
{
    public interface ISettingsProvider
    {
        AppSettings AppSettings { get; }
        PropagateSettings PropagateSettings { get; }
        string PropagateSettingsString { get; }

        bool SearchHttpBody { get; }
        bool AllowVolatileDecorators { get; }
        bool AllowDecoratorsKeyMatch { get; }

        void SetData(AppSettings appSettings, string propagateSettings);
        bool IsTypeAllowed(Type type);
        List<string> GetDecorators();
    }
}