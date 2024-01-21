namespace InvestmentPortfolio.Models;

public record GeolocationDto
(
    string IpAddress,
    string City,
    string Country,
    string Isp,
    string LocalDate,
    string Referer
);