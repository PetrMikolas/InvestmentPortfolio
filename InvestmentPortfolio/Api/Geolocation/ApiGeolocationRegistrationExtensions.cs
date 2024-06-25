using InvestmentPortfolio.Models;
using InvestmentPortfolio.Queries.Geolocation;
using MediatR;

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
        app.MapGet("geolocations", async (IMediator mediator, CancellationToken cancellationToken) =>
        {
            var query = new GetGeolocationsQuery();
            var geolocations = await mediator.Send(query, cancellationToken);
            return Results.Ok(geolocations);
        })
        .WithTags("Geolocations")
        .WithName("GetGeolocations")
        .WithOpenApi(operation => new(operation) { Summary = "Get all geolocations" })
        .Produces<IEnumerable<GeolocationDto>>(StatusCodes.Status200OK);

        return app;
    }
}