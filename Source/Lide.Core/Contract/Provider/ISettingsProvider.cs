using System;
using System.Collections.Generic;
using Lide.Core.Model.Settings;

namespace Lide.Core.Contract.Provider
{
    public interface ISettingsProvider
    {
        AppSettings AppSettings { get; }
        PropagateSettings PropagateSettings { get; }
        PropagateHeaders PropagateHeaders { get; }
        bool AllowVolatileDecorators { get; }
        bool AllowReadonlyDecorators { get; }

        void Initialize(AppSettings appSettings, PropagateSettings propagateSettings, int depth);
        ISet<string> GetDecorators(Type type);
        bool IsAddressAllowed(string address);
    }
}