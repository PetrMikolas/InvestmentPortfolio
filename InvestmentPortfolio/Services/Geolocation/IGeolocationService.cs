namespace InvestmentPortfolio.Services.Geolocation;

/// <summary>
/// Interface for retrieving geolocation data based on IP address.
/// </summary>
public interface IGeolocationService
{
    /// <summary>
    /// Asynchronously retrieves geolocation data based on the provided IP address.
    /// </summary>
    /// <param name="ipAddress">The IP address to retrieve geolocation data for.</param>
    /// <param name="userAgent">The User-Agent header value sent by the client.</param>
    /// <param name="referer">The referer URL.</param>
    /// <param name="cancellationToken">The cancellation token (optional). Defaults to <see cref="CancellationToken.None"/>.</param>
    Task GetGeolocationAsync(string ipAddress, string userAgent, string referer, CancellationToken cancellationToken = default);
}