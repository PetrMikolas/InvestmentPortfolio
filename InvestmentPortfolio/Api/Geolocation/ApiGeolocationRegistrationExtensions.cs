using AutoMapper;
using InvestmentPortfolio.Models;
using InvestmentPortfolio.Repositories.Geolocation;

namespace InvestmentPortfolio.Api.Geolocation;

public static class ApiGeolocationRegistrationExtensions
{
    public static WebApplication MapEndpointsGeolocation(this WebApplication app)
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