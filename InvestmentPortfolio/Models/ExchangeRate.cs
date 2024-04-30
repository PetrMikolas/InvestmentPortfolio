namespace InvestmentPortfolio.Models;

/// <summary>
/// Represents an exchange rate for a specific currency.
/// </summary>
public sealed class ExchangeRate
{
    /// <summary>
    /// Gets or sets the country associated with the currency.
    /// </summary>
    public string Country { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the currency code.
    /// </summary>
    public string Currency { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the ISO currency code.
    /// </summary>
    public string Code { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the amount of currency.
    /// </summary>
    public int Amount { get; set; }

    /// <summary>
    /// Gets or sets the exchange rate.
    /// </summary>
    public float Rate { get; set; }
}