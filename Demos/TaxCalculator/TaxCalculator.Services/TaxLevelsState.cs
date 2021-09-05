using System;
using System.Collections.Generic;
using System.Linq;
using TaxCalculator.Services.Contracts;
using TaxCalculator.Services.Model;

namespace TaxCalculator.Services
{
    public class TaxLevelsState : ITaxLevelsState
    {
        private readonly IList<TaxLevel> _appliedTaxes;

        public TaxLevelsState()
        {
            _appliedTaxes = new List<TaxLevel>();
        }

        public void AddTaxLevel(TaxLevel taxLevel)
        {
            _appliedTaxes.Add(taxLevel);
        }

        public List<TaxLevel> GetTaxes()
        {
            return _appliedTaxes.ToList();
        }

        public void RemoveTaxLevel(string name)
        {
            var taxLevel = _appliedTaxes.FirstOrDefault(x => x.Name == name);
            if (taxLevel != null)
            {
                _appliedTaxes.Remove(taxLevel);
            }
        }

        public string GetInfo()
        {
            var taxesInfo = string.Join(Environment.NewLine, _appliedTaxes.Select(x => x.GetInfo()));
            return $"Calculator is configured with the following taxes:{Environment.NewLine}{taxesInfo}";
        }
    }
}
