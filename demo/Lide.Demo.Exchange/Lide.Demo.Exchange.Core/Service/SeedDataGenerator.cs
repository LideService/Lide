using System;
using System.Collections.Generic;
using Lide.Demo.Exchange.Core.Contracts;
using Lide.Demo.Exchange.Model;

namespace Lide.Demo.Exchange.Core.Service;

public class SeedDataGenerator : ISeedDataGenerator
{
    private readonly IRandomFacade _randomFacade;

    public SeedDataGenerator(IRandomFacade randomFacade)
    {
        _randomFacade = randomFacade;
    }

    public Currency[] GetPredefinedCurrencies()
    {
        return new[]
        {
            new Currency { Name = "Euro", Code = "EUR" },
            new Currency { Name = "US Dollar", Code = "USD" },
            new Currency { Name = "British Pound", Code = "GBP" },
            new Currency { Name = "Japanese Yen", Code = "JPY" },
            new Currency { Name = "Swiss Franc", Code = "CHF" },
            new Currency { Name = "Canadian Dollar", Code = "CAD" },
            new Currency { Name = "Australian Dollar", Code = "AUD" },
            new Currency { Name = "New Zealand Dollar", Code = "NZD" },
            new Currency { Name = "Swedish Krona", Code = "SEK" },
            new Currency { Name = "Norwegian Krone", Code = "NOK" },
            new Currency { Name = "Danish Krone", Code = "DKK" },
            new Currency { Name = "Hong Kong Dollar", Code = "HKD" },
            new Currency { Name = "South African Rand", Code = "ZAR" },
            new Currency { Name = "Mexican Peso", Code = "MXN" },
            new Currency { Name = "Singapore Dollar", Code = "SGD" },
            new Currency { Name = "Chinese Yuan", Code = "CNY" },
            new Currency { Name = "Polish Zloty", Code = "PLN" },
            new Currency { Name = "Russian Ruble", Code = "RUB" },
            new Currency { Name = "Turkish Lira", Code = "TRY" },
            new Currency { Name = "Indian Rupee", Code = "INR" },
            new Currency { Name = "Brazilian Real", Code = "BRL" },
            new Currency { Name = "South Korean Won", Code = "KRW" },
            new Currency { Name = "New Taiwan Dollar", Code = "TWD" },
            new Currency { Name = "Thai Baht", Code = "THB" },
            new Currency { Name = "Indonesian Rupiah", Code = "IDR" },
        };
    }

    public Currency[] GetAnotherSetOfCurrencies()
    {
        return new[]
        {
            new Currency { Name = "Argentine Peso", Code = "ARS" },
            new Currency { Name = "Bulgarian Lev", Code = "BGN" },
            new Currency { Name = "Croatian Kuna", Code = "HRK" },
            new Currency { Name = "Czech Koruna", Code = "CZK" },
            new Currency { Name = "Hungarian Forint", Code = "HUF" },
            new Currency { Name = "Israeli New Shekel", Code = "ILS" },
            new Currency { Name = "Romanian Leu", Code = "RON" },
            new Currency { Name = "Chilean Peso", Code = "CLP" },
            new Currency { Name = "Philippine Peso", Code = "PHP" },
            new Currency { Name = "Ukrainian Hryvnia", Code = "UAH" },
            new Currency { Name = "Colombian Peso", Code = "COP" },
            new Currency { Name = "Peruvian Sol", Code = "PEN" },
            new Currency { Name = "Pakistani Rupee", Code = "PKR" },
        };
    }

    public ExchangeData[] GetExchangeRates(Currency fromCurrency, Currency toCurrency, DateTime startDate, DateTime endDate)
    {
        var exchangeRates = new List<ExchangeData>();
        var numberOfDays = (endDate - startDate).Days;
        for (var i = 0; i < numberOfDays; i++)
        {
            var exchangeDate = startDate.AddDays(i);
            var rate = _randomFacade.GetRandomDecimal(0.5m, 4m);
            if (_randomFacade.NextInt(0, 100) < 10)
            {
                rate = _randomFacade.GetRandomDecimal(10, 30);
            }

            if (_randomFacade.NextInt(0, 100) < 2)
            {
                rate = _randomFacade.GetRandomDecimal(100, 300);
            }

            var exchangeRate = new ExchangeData
            {
                FromCurrency = fromCurrency,
                ToCurrency = toCurrency,
                ExchangeDate = exchangeDate,
                ExchangeRate = rate,
            };

            exchangeRates.Add(exchangeRate);

            if (_randomFacade.NextInt(0, 100) < 10)
            {
                var reverseRate = new ExchangeData
                {
                    FromCurrency = toCurrency,
                    ToCurrency = fromCurrency,
                    ExchangeDate = exchangeDate,
                    ExchangeRate = 1m / rate,
                };

                exchangeRates.Add(reverseRate);
            }
        }

        return exchangeRates.ToArray();
    }
}
