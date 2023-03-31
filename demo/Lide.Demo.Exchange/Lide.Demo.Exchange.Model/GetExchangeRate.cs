using System;

namespace Lide.Demo.Exchange.Model;

public class GetExchangeRate
{
    required public Currency FromCurrency { get; set; }
    required public Currency ToCurrency { get; set; }
    required public DateTime? ExchangeDate { get; set; }
    required public DateTime? StartDate { get; set; }
    required public DateTime? EndDate { get; set; }
}