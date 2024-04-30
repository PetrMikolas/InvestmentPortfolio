using AutoMapper;
using InvestmentPortfolio.Models;
using InvestmentPortfolio.Repositories.Entities;
using InvestmentPortfolio.Repositories.Geolocation;
using InvestmentPortfolio.Services.Email;
using System.Text.Json;

namespace InvestmentPortfolio.Services.Geolocation;

/// <summary>
/// Service for retrieving geolocation data based on IP address, implementing the <see cref="IGeolocationService"/> interface.
/// </summary>
internal sealed class GeolocationService(HttpClient httpClient, IGeolocationRepository geolocationRepository, IMapper mapper, IEmailService email) : IGeolocationService
{
    /// <summary>
    /// Asynchronously retrieves geolocation data based on the provided IP address.
    /// </summary>
    /// <param name="ipAddress">The IP address to retrieve geolocation data for.</param>
    /// <param name="referer">The referer URL (optional). Defaults to an empty string.</param>
    /// <param name="cancellationToken">The cancellation token (optional). Defaults to <see cref="CancellationToken.None"/>.</param>
    public async Task GetGeolocationAsync(string ipAddress, string referer = "", CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrEmpty(ipAddress) || ipAddress.Contains("192.168.1."))
        {
            return;
        }

        var response = await httpClient.GetAsync($"http://ip-api.com/json/{ipAddress}", cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            return;
        }

        string responseContent = await response.Content.ReadAsStringAsync();
        var options = new JsonSerializerOptions() { PropertyNameCaseInsensitive = true };
        var geolocation = JsonSerializer.Deserialize<GeolocationApi>(responseContent, options);

        if (geolocation is null)
        {
            return;
        }

        var geolocationEntity = mapper.Map<GeolocationEntity>(geolocation);
        geolocationEntity.LocalDate = DateTime.Now.ToString();
        geolocationEntity.Referer = referer;

        _ = email.SendObjectAsync(geolocationEntity, "Investiční portfolio - geolocation");
        await geolocationRepository.CreateAsync(geolocationEntity, cancellationToken);
    }
}