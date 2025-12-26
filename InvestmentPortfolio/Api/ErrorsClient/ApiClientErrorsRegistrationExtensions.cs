using InvestmentPortfolio.Helpers;
using InvestmentPortfolio.Services.Email;
using Microsoft.AspNetCore.Mvc;

namespace InvestmentPortfolio.Api.ErrorsClient;

/// <summary>
/// Extension method for registering client errors API endpoints.
/// </summary>
public static class ApiClientErrorsRegistrationExtensions
{
    /// <summary>
    /// Maps an endpoint for reporting client errors to the server.
    /// </summary>
    /// <param name="app">The web application.</param>
    /// <returns>The web application with mapped endpoint for reporting errors.</returns>
    public static WebApplication MapClientErrorEndpoints(this WebApplication app)
    {
        app.MapPost("errors", [EndpointSummary("Send an error")] (
            [FromBody] string errorMessage, 
            [FromServices] IEmailService email, 
            CancellationToken cancellationToken) =>
        {
            if (string.IsNullOrEmpty(errorMessage))
            {
                return Results.BadRequest("HTTP Request Body musí obsahovat chybovou zprávu.");
            }

            app.CreateLogger().LogError("Client Error: {ErrorMessage}", errorMessage);            
            _ = email.SendErrorAsync(errorMessage, cancellationToken);

            return Results.NoContent();
        })
        .WithTags("Errors")
        .WithName("SendError")        
        .Produces(StatusCodes.Status204NoContent)
        .Produces(StatusCodes.Status400BadRequest);

        return app;
    }
}