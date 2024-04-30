namespace InvestmentPortfolio.Models;

/// <summary>
/// Represents a data transfer object (DTO) for geolocation information.
/// </summary>
/// <param name="IpAddress">The IP address.</param>
/// <param name="City">The city.</param>
/// <param name="Country">The country.</param>
/// <param name="Isp">The Internet Service Provider (ISP).</param>
/// <param name="LocalDate">The local date.</param>
/// <param name="Referer">The referer.</param>
public record GeolocationDto
(
    string IpAddress,
    string City,
    string Country,
    string Isp,
    string LocalDate,
    string Referer
);