using AutoMapper;
using InvestmentPortfolio.Models;
using InvestmentPortfolio.Repositories.Geolocation;

namespace InvestmentPortfolio.Api.Geolocation;

/// <summary>
/// Extension method for registering geolocation API endpoints.
/// </summary>
public static class ApiGeolocationRegistrationExtensions
{
    /// <summary>
    /// Maps the endpoints of the geolocations.
    /// </summary>
    /// <param name="app">The WebApplication instance.</param>
    /// <returns>The web application with mapped endpoints for geolocation operations.</returns>
    public static WebApplication MapEndpointsGeolocations(this WebApplication app)
    {
        app.MapGet("geolocations", async (IGeolocationRepository repository, IMapper mapper, CancellationToken cancellationToken) =>
        {
            var result = await repository.GetAllAsync(cancellationToken);
            return Results.Ok(result.Select(geolocation => mapper.Map<GeolocationDto>(geolocation)));
        })
        .WithTags("Geolocations")
        .WithName("GetGeolocations")
        .WithOpenApi(operation => new(operation) { Summary = "Get all geolocations" })
        .Produces<List<GeolocationDto>>(StatusCodes.Status200OK);

        return app;
    }
}