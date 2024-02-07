using InvestmentPortfolio.Models;
using InvestmentPortfolio.Exceptions;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using InvestmentPortfolio.Services.Investment;
using InvestmentPortfolio.Repositories.Entities;
using InvestmentPortfolio.Helpers;

namespace InvestmentPortfolio.Api.Investments;

public static class ApiInvestmentsRegistrationExtensions
{
    public static WebApplication MapEndpointsInvestments(this WebApplication app)
    {
        app.MapGet("investments", async ([FromServices] IInvestmentService investmentsService, [FromServices] IMapper mapper, CancellationToken cancellationToken, [FromQuery(Name = "RefreshExchangeRates")] bool hasRefresExchangeRates = false) =>
        {
            var result = await investmentsService.GetAllAsync(hasRefresExchangeRates, cancellationToken);
            return Results.Ok(mapper.Map<InvestmentsDto>(result));
        })
        .WithTags("Investments")
        .WithName("GetInvestments")
        .WithOpenApi(operation => new(operation) { Summary = "Get all investments" })
        .Produces<InvestmentsDto>(StatusCodes.Status200OK);

        app.MapPost("investments", async ([FromBody] InvestmentDto investmentDto, [FromServices] IInvestmentService investmentsService, [FromServices] IMapper mapper, CancellationToken cancellationToken) =>
        {
            if (!Helper.IsValidInvestmentDto(investmentDto, out string error))
            {
                return Results.BadRequest(error);
            }
            
            var entity = mapper.Map<InvestmentEntity>(investmentDto);
            await investmentsService.CreateAsync(entity, cancellationToken);
            return Results.Created($"investments/{entity.Id}", investmentDto);
        })
        .WithTags("Investments")
        .WithName("CreateInvestment")
        .WithOpenApi(operation => new(operation) { Summary = "Create an investment" })
        .Produces<InvestmentsDto>(StatusCodes.Status201Created)
        .Produces(StatusCodes.Status400BadRequest);

        app.MapPut("investments", async ([FromBody] InvestmentDto investmentDto, [FromServices] IInvestmentService investmentsService, [FromServices] IMapper mapper, CancellationToken cancellationToken) =>
        {
            if (investmentDto.Id <= 0)
            {
                return Results.BadRequest("ID musí být větší než nula.");
            }

            if (!Helper.IsValidInvestmentDto(investmentDto, out string error))
            {
                return Results.BadRequest(error);
            }

            try
            {
                var entita = mapper.Map<InvestmentEntity>(investmentDto);
                await investmentsService.UpdateAsync(entita, cancellationToken);
                return Results.NoContent();
            }
            catch (EntityNotFoundException)
            {
                return Results.NotFound();
            }
        })
        .WithTags("Investments")
        .WithName("UpdateInvestment")
        .WithOpenApi(operation => new(operation) { Summary = "Update the investment" })
        .Produces(StatusCodes.Status204NoContent)
        .Produces(StatusCodes.Status400BadRequest)
        .Produces(StatusCodes.Status404NotFound);

        app.MapDelete("investments/{id}", async ([FromRoute] int id, [FromServices] IInvestmentService investmentsService, CancellationToken cancellationToken) =>
        {
            if (id <= 0)
            {
                return Results.BadRequest("ID musí být větší než nula.");
            }

            try
            {
                await investmentsService.DeleteAsync(id, cancellationToken);
                return Results.NoContent();
            }
            catch (EntityNotFoundException)
            {
                return Results.NotFound();
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