using InvestmentPortfolio.Exceptions;
using InvestmentPortfolio.Helpers;
using InvestmentPortfolio.Repositories.Investment;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace InvestmentPortfolio.Database.Investment;

/// <summary>
/// Provides extension method for configuring database services related to investments.
/// </summary>
public static class InvestmentDatabaseRegistrationExtensions
{
    /// <summary>
    /// Adds database services related to investments to the specified <see cref="IServiceCollection"/>.
    /// </summary>
    /// <param name="services">The collection of services in the application.</param>
    /// <param name="configuration">The configuration of the application.</param>   
    /// <returns>The collection of services with added database services.</returns>
    public static IServiceCollection AddInvestmentDatabase(this IServiceCollection services, IConfiguration configuration)
    {
        if (configuration["environment"] is not "IntegrationTests")
        {
            var connectionString = configuration.GetConnectionString("Investment");

            if (string.IsNullOrWhiteSpace(connectionString))
                throw new ConnectionStringNotFoundException("Nelze získat connection string na připojení databáze Investment");

            services.AddDbContext<InvestmentDbContext>(options =>
            {
                options.UseSqlServer(connectionString, opts =>
                {
                    opts.MigrationsHistoryTable("MigrationHistory_Investment");
                });
            });
        }

        services.RemoveAll<IInvestmentRepository>();
        services.AddScoped<IInvestmentRepository, InvestmentRepository>();

        return services;
    }

    /// <summary>
    /// Applies pending migrations to the database.
    /// </summary>
    /// <param name="app">The web application instance.</param>
    /// <returns>The configured web application instance.</returns>
    public static async Task<WebApplication> UseInvestmentDatabase(this WebApplication app)
    {
        var isRunningAutomatedTest = app.ParseBoolEnvironmentVariable("IS_RUNNING_AUTOMATED_TEST");

        if (app.Environment.EnvironmentName is not "IntegrationTests" && !isRunningAutomatedTest)
        {
            using var scope = app.Services.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<InvestmentDbContext>();

            try
            {
                await dbContext.Database.MigrateAsync(app.Lifetime.ApplicationStopping);
            }
            catch (Exception ex)
            {
                app.CreateLogger().LogError(ex, "Database migration failed");
                throw;
            }
        }

        return app;
    }
}