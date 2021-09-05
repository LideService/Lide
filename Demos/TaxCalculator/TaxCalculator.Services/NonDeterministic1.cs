using TaxCalculator.Services.Contracts;
using TaxCalculator.Services.Model;

namespace TaxCalculator.Services
{
    public class NonDeterministic1 : INonDeterministic1
    {
        private readonly INonDeterministic2 _nonDeterministic2;

        public NonDeterministic1(INonDeterministic2 nonDeterministic2)
        {
            _nonDeterministic2 = nonDeterministic2;
        }

        public BadDesignData InitializeAndDoStuff_NoIsolation(decimal input)
        {
            var data = new BadDesignData()
            {
                Field1 = input * 3 + 13,
                Field2 = input + 19,
            };

            _nonDeterministic2.Volatile_TreatInputAsNonMutable_NoIsolation(data);
            data.Field1 += 1;
            data.Field2 += 2;
            return data;
        }

        public BadDesignData InitializeAndDoStuff_WithIsolation(decimal input)
        {
            var data = new BadDesignData()
            {
                Field1 = input * 3 + 13,
                Field2 = input + 19,
            };

            _nonDeterministic2.Volatile_TreatInputAsNonMutable_WithIsolation(data);
            data.Field1 += 1;
            data.Field2 += 2;
            return data;
        }
    }
}