using InvestmentPortfolio.Database.Investment;
using InvestmentPortfolio.Helpers;
using InvestmentPortfolio.Repositories;
using InvestmentPortfolio.Repositories.Investment;
using InvestmentPortfolio.Services.Email;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace InvestmentPortfolio.Database;

/// <summary>
/// Provides extension method for configuring database services related to investments.
/// </summary>
public static class DatabaseInvestmentRegistrationExtensions
{
    /// <summary>
    /// Adds database services related to investments to the specified <see cref="IServiceCollection"/>.
    /// </summary>
    /// <param name="services">The collection of services in the application.</param>
    /// <param name="configuration">The configuration of the application.</param>
    /// <param name="email">The service for sending email notifications.</param>
    /// <returns>The collection of services with added database services.</returns>
    public static IServiceCollection AddDatabaseInvestment(this IServiceCollection services, IConfiguration configuration, IEmailService email)
    {
        var connectionString = configuration.GetConnectionString("Investment");

        if (string.IsNullOrEmpty(connectionString))
        {            
            string errorMessage = "Nelze získat connection string na připojení databáze Investment";
            Type classType = typeof(DatabaseInvestmentRegistrationExtensions);

            _ = email.SendErrorAsync(errorMessage, classType, nameof(AddDatabaseInvestment));
            LoggerFactory.Create(builder => builder.AddConsole()).CreateLogger(classType).LogError(errorMessage);

            return services;
        }

        services.AddDbContext<InvestmentDbContext>(options =>
        {
            options.UseSqlServer(connectionString, opts =>
            {                
                opts.MigrationsHistoryTable("MigrationHistory_Investment");
            });
        });

        services.AddDatabaseDeveloperPageExceptionFilter();
        services.RemoveAll<IInvestmentRepository>();
        services.AddScoped<IInvestmentRepository, InvestmentRepository>();

        return services;
    }

    /// <summary>
    /// Configures the application to use the investment database.
    /// </summary>
    /// <param name="app">The web application instance.</param>
    /// <param name="email">The service for sending email notifications.</param>
    /// <returns>The configured web application instance.</returns>
    public static WebApplication UseDatabaseInvestment(this WebApplication app, IEmailService email)
    {
        var isRunningAutomatedTest = Helper.ParseBoolEnvironmentVariable("IS_RUNNING_AUTOMATED_TEST");

        if (app.Environment.EnvironmentName != "IntegrationTests" && !isRunningAutomatedTest)
        {
            try
            {
                using var scope = app.Services.CreateScope();
                var dbContext = scope.ServiceProvider.GetRequiredService<InvestmentDbContext>();                
                dbContext.Database.Migrate();
            }
            catch (Exception ex)
            {
                _ = email.SendErrorAsync(ex.ToString());
                app.Logger.LogError(ex.ToString());

                return app;
            }
        }

        return app;
    }
}