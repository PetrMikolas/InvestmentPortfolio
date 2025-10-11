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
/// <param name="httpClient">The HTTP client used for making requests to the geolocation API.</param>
/// <param name="geolocationRepository">The repository for managing geolocation data.</param>
/// <param name="mapper">The mapper for mapping between geolocation models and entities.</param>
/// <param name="email">The service for sending email notifications.</param>
/// <param name="logger">The logger instance for logging errors.</param>
internal sealed class GeolocationService(HttpClient httpClient, IGeolocationRepository geolocationRepository, IMapper mapper, IEmailService email, ILogger<GeolocationService> logger) : IGeolocationService
{
    public async Task GetGeolocationAsync(string ipAddress, string userAgent, string referer, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrEmpty(ipAddress) || ipAddress.Contains("192.168.1."))
        {
            return;
        }

        try
        {
            var response = await httpClient.GetAsync($"http://ip-api.com/json/{ipAddress}", cancellationToken);

            if (!response.IsSuccessStatusCode)
            {
                return;
            }

            string responseContent = await response.Content.ReadAsStringAsync(cancellationToken);
            var options = new JsonSerializerOptions() { PropertyNameCaseInsensitive = true };
            var geolocation = JsonSerializer.Deserialize<GeolocationApi>(responseContent, options);

            if (geolocation is null)
            {
                return;
            }

            var geolocationEntity = mapper.Map<GeolocationEntity>(geolocation);
            geolocationEntity.LocalDate = DateTime.Now.ToString();
            geolocationEntity.UserAgent = userAgent;
            geolocationEntity.Referer = referer;

            await email.SendObjectAsync(geolocationEntity, "Investiční portfolio - geolocation", cancellationToken);
            await geolocationRepository.CreateAsync(geolocationEntity, cancellationToken);
        }
        catch (Exception ex)
        {            
            logger.LogError(ex.ToString());
        }
    }
}