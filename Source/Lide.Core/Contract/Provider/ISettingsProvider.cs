using System;
using System.Collections.Generic;
using Lide.Core.Model.Settings;

namespace Lide.Core.Contract.Provider
{
    public interface ISettingsProvider
    {
        AppSettings AppSettings { get; }
        PropagateSettings PropagateSettings { get; }
        public int Depth { get; set; }
        public int NextDepth { get; }
        bool AllowVolatileDecorators { get; }
        bool AllowReadonlyDecorators { get; }
        string OriginRequestPath { get; set; }

        void Initialize(AppSettings appSettings, PropagateSettings propagateSettings);
        ISet<string> GetDecorators(Type type);
        bool IsAddressAllowed(string address);
    }
}