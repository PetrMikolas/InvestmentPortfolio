namespace InvestmentPortfolio.Models;

public sealed class Investment
{
    public int Id { get; set; }

    public string Name { get; set; } = string.Empty;
 
    public int Value { get; set; }        

    public string CurrencyCode { get; set; } = string.Empty;

    public long ValueCzk { get; set; }

    public string Percentage { get; set; } = string.Empty;

    public string NamePercentage => $"{Name} ({Percentage})";
}