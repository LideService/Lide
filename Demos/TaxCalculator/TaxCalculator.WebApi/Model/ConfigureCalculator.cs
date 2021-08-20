using System.Collections.Generic;
using TaxCalculator.Services.Model;

namespace TaxCalculator.WebApi.Model
{
    public class ConfigureCalculator
    {
        public string Name { get; set; }
        public List<TaxLevel> TaxLevels { get; set; }
        public bool Override { get; set; }
    }
}