using System;
using System.Linq;
using System.Threading;
using Lide.Demo.Exchange.Core.Contracts;
using Lide.Demo.Exchange.DAL.Contract;
using Lide.Demo.Exchange.Model;

namespace Lide.Demo.Exchange.Core.Service;

public class ExchangeService : IExchangeService
{
    private static int _id;
    private readonly IExchangeRepository _exchangeRepository;
    private readonly IMapper _mapper;

    public ExchangeService(
        IExchangeRepository exchangeRepository,
        IMapper mapper)
    {
        _exchangeRepository = exchangeRepository;
        _mapper = mapper;
    }

    public void AddCurrency(Currency currency)
    {
        var id = Interlocked.Increment(ref _id);
        _exchangeRepository.AddCurrency(_mapper.MapFromModel(id, currency));
    }

    public void AddExchangeRate(ExchangeData exchangeRate)
    {
        var id = Interlocked.Increment(ref _id);
        var fromCurrency = _exchangeRepository.GetCurrency(exchangeRate.FromCurrency.Code);
        var toCurrency = _exchangeRepository.GetCurrency(exchangeRate.ToCurrency.Code);
        if (fromCurrency == null || toCurrency == null)
        {
            throw new Exception("Currency not found");
        }

        _exchangeRepository.AddExchangeRate(_mapper.MapFromModel(id, exchangeRate, fromCurrency.Id, toCurrency.Id));
    }

    public Currency? GetCurrency(string code)
    {
        var currency = _exchangeRepository.GetCurrency(code);
        return currency == null ? null : _mapper.MapFromDAL(currency);
    }

    public ExchangeData? GetExchangeRateWithThirdOptional(string fromCode, string toCode, DateTime exchangeDate)
    {
        var fromCurrency = _exchangeRepository.GetCurrency(fromCode);
        var toCurrency = _exchangeRepository.GetCurrency(toCode);
        if (fromCurrency == null || toCurrency == null)
        {
            throw new Exception("Currency not found");
        }

        var exchangeRate = _exchangeRepository.GetExchangeRate(fromCode, toCode, exchangeDate);
        if (exchangeRate != null)
        {
            return _mapper.MapFromDAL(exchangeRate, fromCurrency, toCurrency);
        }

        exchangeRate = _exchangeRepository.GetExchangeRate(toCode, fromCode, exchangeDate);
        if (exchangeRate != null)
        {
            return _mapper.MapFromDAL(exchangeRate, fromCurrency, toCurrency);
        }

        var thirdCurrency = _exchangeRepository.GetThirdCurrency(fromCode, toCode, exchangeDate);
        if (thirdCurrency != null)
        {
            var fromToThird = _exchangeRepository.GetExchangeRate(fromCode, thirdCurrency.Code, exchangeDate);
            var thirdToTo = _exchangeRepository.GetExchangeRate(thirdCurrency.Code, toCode, exchangeDate);
            if (fromToThird != null && thirdToTo != null)
            {
                return new ExchangeData
                {
                    FromCurrency = _mapper.MapFromDAL(fromCurrency),
                    ToCurrency = _mapper.MapFromDAL(toCurrency),
                    ExchangeRate = fromToThird.ExchangeRate * thirdToTo.ExchangeRate,
                    ExchangeDate = exchangeDate,
                };
            }
        }

        thirdCurrency = _exchangeRepository.GetThirdCurrency(toCode, fromCode, exchangeDate);
        if (thirdCurrency != null)
        {
            var toToThird = _exchangeRepository.GetExchangeRate(toCode, thirdCurrency.Code, exchangeDate);
            var thirdToFrom = _exchangeRepository.GetExchangeRate(thirdCurrency.Code, fromCode, exchangeDate);
            if (toToThird != null && thirdToFrom != null)
            {
                return new ExchangeData
                {
                    FromCurrency = _mapper.MapFromDAL(fromCurrency),
                    ToCurrency = _mapper.MapFromDAL(toCurrency),
                    ExchangeRate = 1 / (toToThird.ExchangeRate * thirdToFrom.ExchangeRate),
                    ExchangeDate = exchangeDate,
                };
            }
        }

        return null;
    }

    public ExchangeData[] GetExchangeRates(Currency fromCurrency, Currency toCurrency)
    {
        var dalFromCurrency = _exchangeRepository.GetCurrency(fromCurrency.Code);
        var dalToCurrency = _exchangeRepository.GetCurrency(toCurrency.Code);
        if (dalFromCurrency == null || dalToCurrency == null)
        {
            throw new Exception("Currency not found");
        }

        var rates = _exchangeRepository.GetExchangeRates(fromCurrency.Code, toCurrency.Code, null, null);
        return rates.Select(r => _mapper.MapFromDAL(r, dalFromCurrency, dalToCurrency)).ToArray();
    }

    public ExchangeData[] GetExchangeRates(Currency fromCurrency, Currency toCurrency, DateTime startDate, DateTime endDate)
    {
        var dalFromCurrency = _exchangeRepository.GetCurrency(fromCurrency.Code);
        var dalToCurrency = _exchangeRepository.GetCurrency(toCurrency.Code);
        if (dalFromCurrency == null || dalToCurrency == null)
        {
            throw new Exception("Currency not found");
        }

        var rates = _exchangeRepository.GetExchangeRates(fromCurrency.Code, toCurrency.Code, startDate, endDate);
        return rates.Select(r => _mapper.MapFromDAL(r, dalFromCurrency, dalToCurrency)).ToArray();
    }
}
