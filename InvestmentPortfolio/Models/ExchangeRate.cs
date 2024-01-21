namespace InvestmentPortfolio.Models;

public sealed class ExchangeRate
{
	public string Country { get; set; } = string.Empty;
	public string Currency { get; set; } = string.Empty;
	public string Code { get; set; } = string.Empty; 
	public int Amount { get; set; }
	public float Rate { get; set; }
}