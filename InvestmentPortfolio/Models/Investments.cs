namespace InvestmentPortfolio.Models;

/// <summary>
/// Represents a model for investments.
/// </summary>
public sealed class Investments
{
    /// <summary>
    /// Gets or sets the total sum in CZK.
    /// </summary>
    public long TotalSumCzk { get; set; }

    /// <summary>
    /// Gets or sets the total performance in CZK.
    /// </summary>
    public int TotalPerformanceCzk { get; set; }

    /// <summary>
    /// Gets or sets the total performance percentage.
    /// </summary>
    public float TotalPerformancePercentage { get; set; }

    /// <summary>
    /// Gets or sets the list of investment items.
    /// </summary>
    public List<Investment> Items { get; set; } = [];

    /// <summary>
    /// Gets or sets the exchange rates.
    /// </summary>
    public ExchangeRates ExchangeRates { get; set; } = new();
}