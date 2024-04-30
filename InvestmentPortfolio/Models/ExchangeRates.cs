namespace InvestmentPortfolio.Models;

/// <summary>
/// Represents a collection of exchange rates.
/// </summary>
public sealed class ExchangeRates
{
    /// <summary>
    /// Gets or sets the date of the exchange rates.
    /// </summary>
    public string Date { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the list of exchange rates.
    /// </summary>
    public List<ExchangeRate> Items { get; set; } = [];
}