namespace Lide.Demo.Reporting.Model;

public class CalculateRequest
{
    required public decimal Amount { get; set; }
    required public TaxName[] TaxNames { get; set; }
}