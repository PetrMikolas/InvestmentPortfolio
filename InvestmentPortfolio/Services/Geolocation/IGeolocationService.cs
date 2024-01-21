namespace InvestmentPortfolio.Services.Geolocation;

public interface IGeolocationService
{
    Task GetGeolocationAsync(string ipAddress, string referer, CancellationToken cancellationToken = default);
}