namespace InvestmentPortfolio.Models;

public record InvestmentsDto
(
    long TotalSum,
    List<Investment> Items,
    ExchangeRates ExchangeRates
);