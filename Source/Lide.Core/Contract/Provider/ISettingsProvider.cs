using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Lide.Core.Model;
using Lide.Core.Model.Settings;

namespace Lide.Core.Contract.Provider
{
    public interface ISettingsProvider
    {
        AppSettings AppSettings { get; }
        PropagateSettings PropagateSettings { get; }
        string PropagateSettingsString { get; }

        bool SearchHttpBodyOrQuery { get; }
        bool AllowVolatileDecorators { get; }
        bool AllowEnablingDecorators { get; }

        void SetData(AppSettings appSettings, string propagateSettings);
        bool IsTypeAllowed(Type type, string decoratorId);
        List<string> GetDecorators();
    }
}