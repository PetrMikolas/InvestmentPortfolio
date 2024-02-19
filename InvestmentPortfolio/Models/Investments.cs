namespace InvestmentPortfolio.Models;

public class Investments
{
    public long TotalSumCzk { get; set; }

    public int TotalPerformanceCzk { get; set; }

    public float TotalPerformancePercentage { get; set; }

    public List<Investment> Items { get; set; } = [];

    public ExchangeRates ExchangeRates { get; set; } = new();
}