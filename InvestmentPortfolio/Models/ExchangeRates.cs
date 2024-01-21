namespace InvestmentPortfolio.Models;

public sealed class ExchangeRates
{
	public string Date { get; set; } = string.Empty;

	public List<ExchangeRate> Items { get; set; } = [];
}