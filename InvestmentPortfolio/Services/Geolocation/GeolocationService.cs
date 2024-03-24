using AutoMapper;
using InvestmentPortfolio.Models;
using InvestmentPortfolio.Repositories.Entities;
using InvestmentPortfolio.Repositories.Geolocation;
using InvestmentPortfolio.Services.Email;
using System.Text.Json;

namespace InvestmentPortfolio.Services.Geolocation;

public class GeolocationService(HttpClient httpClient, IGeolocationRepository geolocationRepository, IMapper mapper, IEmailService email) : IGeolocationService
{
    public async Task GetGeolocationAsync(string ipAddress, string referer, CancellationToken cancellationToken = default)
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