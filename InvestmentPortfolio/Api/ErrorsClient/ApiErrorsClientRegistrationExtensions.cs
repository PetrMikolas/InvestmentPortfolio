using InvestmentPortfolio.Services.Email;
using Microsoft.AspNetCore.Mvc;

namespace InvestmentPortfolio.Api.ErrorsClient;

public static class ApiErrorsClientRegistrationExtensions
{
    public static WebApplication MapEndpointsErrorsClient(this WebApplication app)
    {
        app.MapPost("errors", ([FromBody] string errorMessage, [FromServices] IEmailService email, CancellationToken cancellationToken) =>
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"Error: {errorMessage}");            
            Console.ResetColor();

            _ = email.SendErrorAsync(errorMessage, cancellationToken);

            return Results.NoContent();
        })
        .WithTags("Errors")
        .WithName("SendError")
        .WithOpenApi(operation => new(operation) { Summary = "Send an error" })
        .Produces(StatusCodes.Status204NoContent);

        return app;
    }
}