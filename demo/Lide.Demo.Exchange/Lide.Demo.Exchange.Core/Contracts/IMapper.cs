using System.Threading;

namespace Lide.Demo.Exchange.Core.Contracts;

public interface IMapper
{
    DAL.Model.Currency MapFromModel(int id, Model.Currency currency);
    DAL.Model.ExchangeData MapFromModel(int id, Model.ExchangeData exchangeRate, int fromCurrencyId, int toCurrencyId);
    Model.ExchangeData MapFromDAL(DAL.Model.ExchangeData exchangeRate, DAL.Model.Currency fromCurrency, DAL.Model.Currency toCurrency);
    Model.Currency MapFromDAL(DAL.Model.Currency fromCurrency);
}