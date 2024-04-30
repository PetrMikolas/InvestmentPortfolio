namespace InvestmentPortfolio.Models;

/// <summary>
/// Data transfer object representing investment information.
/// </summary>
/// <param name="TotalSumCzk">Total sum in CZK.</param>
/// <param name="TotalPerformanceCzk">Total performance in CZK.</param>
/// <param name="TotalPerformancePercentage">Total performance percentage.</param>
/// <param name="Items">List of individual investments.</param>
/// <param name="ExchangeRates">Exchange rates information.</param>
public record InvestmentsDto
(
    long TotalSumCzk,
    int TotalPerformanceCzk,
    float TotalPerformancePercentage,
    List<Investment> Items,
    ExchangeRates ExchangeRates
);