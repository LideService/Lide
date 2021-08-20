namespace TaxCalculator.Services.Model
{
    public class TaxLevel
    {
        public TaxLevel()
        {
        }

        public TaxLevel(string name, decimal? lowerLimit, decimal? upperLimit, decimal percentage)
        {
            Name = name;
            LowerLimit = lowerLimit;
            UpperLimit = upperLimit;
            Percentage = percentage;
        }

        public string Name { get; set; }
        public decimal? LowerLimit { get; set; }
        public decimal? UpperLimit { get; set; }
        public decimal Percentage { get; set; }

        public string GetInfo()
        {
            var info = $"{Name} of {Percentage}% is applied to";
            return LowerLimit switch
            {
                null when !UpperLimit.HasValue => $"{info} all amounts",
                null => $"{info} amounts less than {UpperLimit.Value}",
                _ => !UpperLimit.HasValue
                    ? $"{info} amounts greater than {LowerLimit.Value}"
                    : $"{info} amounts between {LowerLimit.Value} and {UpperLimit.Value}"
            };
        }
    }
}
