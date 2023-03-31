using System;
using Lide.Demo.Exchange.Model;

namespace Lide.Demo.Exchange.Core.Contracts;

public interface ISeedDataGenerator
{
    Currency[] GetAnotherSetOfCurrencies();
    Currency[] GetPredefinedCurrencies();
    ExchangeData[] GetExchangeRates(Currency fromCurrency, Currency toCurrency, DateTime startDate, DateTime endDate);
}