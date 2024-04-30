using System.ComponentModel.DataAnnotations;

namespace InvestmentPortfolio.Models;

/// <summary>
/// Represents data transfer object (DTO) for an investment.
/// </summary>
public class InvestmentDto
{
    /// <summary>
    /// Gets or sets the ID of the investment.
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Gets or sets the name of the investment.
    /// </summary>
    [Required, MaxLength(40, ErrorMessage = "max délka 40 znaků")]
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the value of the investment.
    /// </summary>
    [Required, Range(1, 100000000, ErrorMessage = "rozsah 1 až 100 000 000")]
    public int Value { get; set; }

    /// <summary>
    /// Gets or sets the currency code of the investment.
    /// </summary>
    [Required, StringLength(3, ErrorMessage = "povinný údaj 3 znaky", MinimumLength = 3)]
    public string CurrencyCode { get; set; } = string.Empty;
}