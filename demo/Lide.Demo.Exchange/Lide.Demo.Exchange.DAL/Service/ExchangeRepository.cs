using System;
using System.Collections.Generic;
using System.Linq;
using Lide.Demo.Exchange.DAL.Contract;
using Lide.Demo.Exchange.DAL.Model;

namespace Lide.Demo.Exchange.DAL.Service;

public class ExchangeRepository : IExchangeRepository
{
    private readonly ExchangeContext _context;

    public ExchangeRepository(ExchangeContext context)
    {
        _context = context;
    }

    public void AddCurrency(Currency currency)
    {
        _context.Currencies.Add(currency);
        _context.SaveChanges();
    }

    public void AddExchangeRate(ExchangeData exchangeRate)
    {
        _context.ExchangeRates.Add(exchangeRate);
        _context.SaveChanges();
    }

    public Currency GetCurrency(int id)
    {
        return _context.Currencies.First(c => c.Id == id);
    }

    public Currency? GetCurrency(string code)
    {
        return _context.Currencies.FirstOrDefault(c => c.Code == code);
    }

    public ExchangeData? GetExchangeRate(string fromCode, string toCode, DateTime exchangeDate)
    {
        var fromId = _context.Currencies.FirstOrDefault(c => c.Code == fromCode)?.Id ?? -1;
        var toId = _context.Currencies.FirstOrDefault(c => c.Code == toCode)?.Id ?? -1;
        return _context.ExchangeRates
            .Where(r => r.FromCurrencyId == fromId)
            .Where(r => r.ToCurrencyId == toId)
            .Where(r => r.ExchangeDate == exchangeDate)
            .FirstOrDefault();
    }

    public List<ExchangeData> GetExchangeRates(string fromCode, string toCode, DateTime? fromDate, DateTime? toDate)
    {
        var fromId = _context.Currencies.FirstOrDefault(c => c.Code == fromCode)?.Id ?? -1;
        var toId = _context.Currencies.FirstOrDefault(c => c.Code == toCode)?.Id ?? -1;
        var query = _context.ExchangeRates
            .Where(r => r.FromCurrencyId == fromId)
            .Where(r => r.ToCurrencyId == toId);
        if (fromDate.HasValue)
        {
            query = query.Where(r => r.ExchangeDate >= fromDate.Value);
        }

        if (toDate.HasValue)
        {
            query = query.Where(r => r.ExchangeDate <= toDate.Value);
        }

        return query.ToList();
    }

    public Currency? GetThirdCurrency(string fromCode, string toCode, DateTime exchangeDate)
    {
        var fromId = _context.Currencies.FirstOrDefault(c => c.Code == fromCode)?.Id ?? -1;
        var toId = _context.Currencies.FirstOrDefault(c => c.Code == toCode)?.Id ?? -1;
        var thirdCurrencyQuery =
            from fromCurrency in _context.ExchangeRates
                .Where(x => x.FromCurrencyId == fromId)
                .Where(x => x.ExchangeDate == exchangeDate)
            join toCurrency in _context.ExchangeRates
                .Where(x => x.ToCurrencyId == toId)
                .Where(x => x.ExchangeDate == exchangeDate) on fromCurrency.ToCurrencyId equals toCurrency.FromCurrencyId
            select fromCurrency.ToCurrencyId;

        var thirdCurrency = thirdCurrencyQuery.FirstOrDefault();
        return _context.Currencies.FirstOrDefault(c => c.Id == thirdCurrency);
    }

    public List<Currency> GetCurrencies()
    {
        return _context.Currencies.ToList();
    }
}
