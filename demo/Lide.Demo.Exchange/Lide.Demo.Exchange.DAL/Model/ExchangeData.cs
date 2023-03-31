using System;

namespace Lide.Demo.Exchange.DAL.Model;

public class ExchangeData
{
    required public int Id { get; set; }
    required public int FromCurrencyId { get; set; }
    required public int ToCurrencyId { get; set; }
    required public DateTime ExchangeDate { get; set; }
    required public decimal ExchangeRate { get; set; }
}
