namespace VendingMachine.Services.Model
{
    public class VendingItem
    {
        private static int _nextId = 0;

        public VendingItem(string name, int costInPence)
        {
            Id = ++_nextId;
            Name = name;
            CostInPence = costInPence;
            CostInDecimal = costInPence / (decimal)Constants.PoundToPensMultiplier;
        }

        public int Id { get; }
        public string Name { get; }
        public int CostInPence { get; }
        public decimal CostInDecimal { get; }
    }
}
