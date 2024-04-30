namespace InvestmentPortfolio.Models;

/// <summary>
/// Represents an investment model.
/// </summary>
public sealed class Investment
{
    /// <summary>
    /// Gets or sets the unique identifier of the investment.
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Gets or sets the name of the investment.
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the value of the investment.
    /// </summary>
    public int Value { get; set; }

    /// <summary>
    /// Gets or sets the currency code of the investment.
    /// </summary>
    public string CurrencyCode { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the value of the investment in Czech Koruna (CZK).
    /// </summary>
    public long ValueCzk { get; set; }

    /// <summary>
    /// Gets or sets the percentage share of the investment.
    /// </summary>
    public float PercentageShare { get; set; }

    /// <summary>
    /// Gets a formatted string representing the name and percentage share of the investment.
    /// </summary>
    public string NamePercentageShare => $"{Name} ({PercentageShare} %)";

    /// <summary>
    /// Gets or sets the date and time when the investment was created.
    /// </summary>
    public DateTimeOffset CreatedDate { get; set; }

    /// <summary>
    /// Gets or sets the default value of the investment in Czech Koruna (CZK).
    /// </summary>
    public long DefaultValueCzk { get; set; }

    /// <summary>
    /// Gets or sets the performance of the investment in Czech Koruna (CZK).
    /// </summary>
    public int PerformanceCzk { get; set; }

    /// <summary>
    /// Gets or sets the percentage performance of the investment.
    /// </summary>
    public float PerformancePercentage { get; set; }
}