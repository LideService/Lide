using System;
using Lide.Demo.Exchange.Model;

namespace Lide.Demo.Exchange.Core.Contracts;

public interface IExchangeService
{
    void AddCurrency(Currency currency);
    void AddExchangeRate(ExchangeData exchangeRate);

    Currency? GetCurrency(string code);
    ExchangeData? GetExchangeRateWithThirdOptional(string fromCode, string toCode, DateTime exchangeDate);
    ExchangeData[] GetExchangeRates(Currency fromCurrency, Currency toCurrency);
    ExchangeData[] GetExchangeRates(Currency fromCurrency, Currency toCurrency, DateTime startDate, DateTime endDate);
}