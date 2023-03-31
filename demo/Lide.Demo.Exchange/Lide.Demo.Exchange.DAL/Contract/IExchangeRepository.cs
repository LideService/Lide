using System;
using System.Collections.Generic;
using System.Linq;
using Lide.Demo.Exchange.DAL.Model;

namespace Lide.Demo.Exchange.DAL.Contract;

public interface IExchangeRepository
{
    void AddCurrency(Currency currency);
    void AddExchangeRate(ExchangeData exchangeRate);
    Currency GetCurrency(int id);
    Currency? GetCurrency(string code);
    ExchangeData? GetExchangeRate(string fromCode, string toCode, DateTime exchangeDate);
    List<ExchangeData> GetExchangeRates(string fromCode, string toCode, DateTime? fromDate, DateTime? toDate);
    Currency? GetThirdCurrency(string fromCode, string toCode, DateTime exchangeDate);
    List<Currency> GetCurrencies();
}
