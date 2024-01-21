using System.ComponentModel.DataAnnotations;

namespace InvestmentPortfolio.Models;

public sealed class InvestmentDto
{
    public int Id { get; set; }

    [Required, MaxLength(40, ErrorMessage = "max délka 40 znaků")]
    public string Name { get; set; } = string.Empty;

    [Required, Range(1, 100000000, ErrorMessage = "rozsah 1 až 100 000 000")]
    public int Value { get; set; }

    [Required, StringLength(3, ErrorMessage = "povinný údaj 3 znaky", MinimumLength = 3)]
    public string CurrencyCode { get; set; } = string.Empty;
}