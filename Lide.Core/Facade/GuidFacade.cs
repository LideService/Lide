using System;
using Lide.Core.Contract.Facade;

namespace Lide.Core.Facade
{
    public class GuidFacade : IGuidFacade
    {
        public string NewGuid()
        {
            return Guid.NewGuid().ToString();
        }
    }
}