using System.Threading;
using Lide.Demo.Exchange.Core.Contracts;

namespace Lide.Demo.Exchange.Core.Service;

public class Mapper : IMapper
{
    public DAL.Model.Currency MapFromModel(int id, Model.Currency currency)
    {
        return new DAL.Model.Currency
        {
            Id = id,
            Code = currency.Code,
            Name = currency.Name,
        };
    }

    public DAL.Model.ExchangeData MapFromModel(int id, Model.ExchangeData exchangeRate, int fromCurrencyId, int toCurrencyId)
    {
        return new DAL.Model.ExchangeData
        {
            Id = id,
            FromCurrencyId = fromCurrencyId,
            ToCurrencyId = toCurrencyId,
            ExchangeRate = exchangeRate.ExchangeRate,
            ExchangeDate = exchangeRate.ExchangeDate,
        };
    }

    public Model.ExchangeData MapFromDAL(DAL.Model.ExchangeData exchangeRate, DAL.Model.Currency fromCurrency, DAL.Model.Currency toCurrency)
    {
        return new Model.ExchangeData
        {
            FromCurrency = MapFromDAL(fromCurrency),
            ToCurrency = MapFromDAL(toCurrency),
            ExchangeRate = exchangeRate.ExchangeRate,
            ExchangeDate = exchangeRate.ExchangeDate,
        };
    }

    public Model.Currency MapFromDAL(DAL.Model.Currency fromCurrency)
    {
        return new Model.Currency
        {
            Code = fromCurrency.Code,
            Name = fromCurrency.Name,
        };
    }
}