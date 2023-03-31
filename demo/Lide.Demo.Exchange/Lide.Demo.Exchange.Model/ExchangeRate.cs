using System;

namespace Lide.Demo.Exchange.Model;

public class ExchangeData
{
    required public Currency FromCurrency { get; set; }
    required public Currency ToCurrency { get; set; }
    required public decimal ExchangeRate { get; set; }
    required public DateTime ExchangeDate { get; set; }
}