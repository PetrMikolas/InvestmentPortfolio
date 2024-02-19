namespace InvestmentPortfolio.Models;

public record InvestmentsDto
(
    long TotalSumCzk,
    int TotalPerformanceCzk,
    float TotalPerformancePercentage,
    List<Investment> Items,
    ExchangeRates ExchangeRates
);