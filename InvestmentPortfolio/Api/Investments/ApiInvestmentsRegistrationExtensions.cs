using InvestmentPortfolio.Models;
using InvestmentPortfolio.Exceptions;
using Microsoft.AspNetCore.Mvc;
using InvestmentPortfolio.Helpers;
using MediatR;
using InvestmentPortfolio.Queries.Investment;
using InvestmentPortfolio.Commands.Investment;

namespace InvestmentPortfolio.Api.Investments;

/// <summary>
/// Extension method for registering investment API endpoints.
/// </summary>
public static class ApiInvestmentsRegistrationExtensions
{
    /// <summary>
    /// Maps the endpoints of the investments.
    /// </summary>
    /// <param name="app">The WebApplication instance.</param>
    /// <returns>The web application with mapped endpoints for investment operations.</returns>
    public static WebApplication MapEndpointsInvestments(this WebApplication app)
    {
        app.MapGet("investments", async ([FromQuery(Name = "RefreshExchangeRates")] bool hasRefreshExchangeRates, [FromServices] IMediator mediator, CancellationToken cancellationToken) =>
        {
            var query = new GetInvestmentsQuery(hasRefreshExchangeRates);
            var investments = await mediator.Send(query, cancellationToken);
            return Results.Ok(investments);
        })
        .WithTags("Investments")
        .WithName("GetInvestments")
        .WithOpenApi(operation => new(operation) { Summary = "Get all investments" })
        .Produces<InvestmentsDto>(StatusCodes.Status200OK);

        app.MapPost("investments", async ([FromBody] InvestmentDto investmentDto, [FromServices] IMediator mediator, CancellationToken cancellationToken) =>
        {
            if (!Helper.IsValidInvestmentDto(investmentDto, HttpMethod.Post, out string error))
            {
                return Results.BadRequest(error);
            }

            var command = new CreateInvestmentCommand(investmentDto);
            var cretedInvestmentId = await mediator.Send(command, cancellationToken);
            return Results.Created($"investments/{cretedInvestmentId}", null);
        })
        .WithTags("Investments")
        .WithName("CreateInvestment")
        .WithOpenApi(operation => new(operation) { Summary = "Create an investment" })
        .Produces(StatusCodes.Status201Created)
        .Produces(StatusCodes.Status400BadRequest);

        app.MapPut("investments", async ([FromBody] InvestmentDto investmentDto, [FromServices] IMediator mediator, CancellationToken cancellationToken) =>
        {
            if (!Helper.IsValidInvestmentDto(investmentDto, HttpMethod.Put, out string error))
            {
                return Results.BadRequest(error);
            }

            try
            {
                var command = new UpdateInvestmentCommand(investmentDto);
                await mediator.Send(command, cancellationToken);
                return Results.NoContent();
            }
            catch (EntityNotFoundException ex)
            {
                return Results.NotFound(ex.Message);
            }
        })
        .WithTags("Investments")
        .WithName("UpdateInvestment")
        .WithOpenApi(operation => new(operation) { Summary = "Update the investment" })
        .Produces(StatusCodes.Status204NoContent)
        .Produces(StatusCodes.Status400BadRequest)
        .Produces(StatusCodes.Status404NotFound);

        app.MapDelete("investments/{id}", async ([FromRoute] int id, [FromServices] IMediator mediator, CancellationToken cancellationToken) =>
        {
            if (id <= 0)
            {
                return Results.BadRequest($"Parametr <{nameof(id)}> musí mít větší hodnotu než nula");
            }

            try
            {
                var command = new DeleteInvestmentCommand(id);
                await mediator.Send(command, cancellationToken);
                return Results.NoContent();
            }
            catch (EntityNotFoundException ex)
            {
                return Results.NotFound(ex.Message);
            }
        })
        .WithTags("Investments")
        .WithName("DeleteInvestment")
        .WithOpenApi(operation => new(operation) { Summary = "Delete investment by ID" })
        .Produces(StatusCodes.Status204NoContent)
        .Produces(StatusCodes.Status400BadRequest)
        .Produces(StatusCodes.Status404NotFound);

        return app;
    }
}