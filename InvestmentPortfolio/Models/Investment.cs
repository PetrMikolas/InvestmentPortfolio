using InvestmentPortfolio.Repositories.Entities;

namespace InvestmentPortfolio.Models;

public sealed class Investment
{
    public int Id { get; set; }

    public string Name { get; set; } = string.Empty;
 
    public int Value { get; set; }        

    public string CurrencyCode { get; set; } = string.Empty;

    public long ValueCzk { get; set; }

    public float PercentageShare { get; set; }

    public string NamePercentageShare => $"{Name} ({PercentageShare} %)";

    public DateTimeOffset CreatedDate { get; set; }
   
    public long DefaultValueCzk { get; set; }

    public int PerformanceCzk { get; set; }

    public float PerformancePercentage { get; set; }
}