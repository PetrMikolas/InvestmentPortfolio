namespace InvestmentPortfolio.Repositories.Entities;

/// <summary>
/// Represents an entity for storing investment information in the database.
/// </summary>
public class InvestmentEntity
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
    /// Gets or sets the default value in Czech Koruna (CZK) of the investment.
    /// </summary>
    public long? DefaultValueCzk { get; set; }

    /// <summary>
    /// Gets or sets the date and time when the investment was created.
    /// </summary>
    public DateTimeOffset CreatedAt { get; set; }

    /// <summary>
    /// Gets or sets the date and time when the investment was last updated.
    /// </summary>
    public DateTimeOffset? UpdatedAt { get; set; }
}