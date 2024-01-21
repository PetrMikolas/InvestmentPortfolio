namespace InvestmentPortfolio.Models;

public class Investments
{
    public long TotalSum { get; set; }

    public List<Investment> Items { get; set; } = [];

    public ExchangeRates ExchangeRates { get; set; } = new();
}