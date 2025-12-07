using InvestmentPortfolio.Helpers;
using InvestmentPortfolio.Repositories.Geolocation;
using InvestmentPortfolio.Services.Email;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace InvestmentPortfolio.Database.Geolocation;

/// <summary>
/// Provides extension method for configuring database services related to geolocation.
/// </summary>
public static class GeolocationDatabaseRegistrationExtensions
{
    /// <summary>
    /// Adds database services related to geolocation to the specified <see cref="IServiceCollection"/>.
    /// </summary>
    /// <param name="services">The collection of services in the application.</param>
    /// <param name="configuration">The configuration of the application.</param>    
    /// <returns>The collection of services with added database services.</returns>
    public static IServiceCollection AddGeolocationDatabase(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<GeolocationDbContext>((serviceProvider, options) =>
        {
            var connectionString = configuration.GetConnectionString("Geolocation");

            if (string.IsNullOrWhiteSpace(connectionString))
            {
                string errorMessage = "Nelze získat connection string na připojení databáze Geolocation";
                serviceProvider.CreateLogger().LogError(errorMessage);
                var email = serviceProvider.GetRequiredService<IEmailService>();
                _ = email.SendErrorAsync(errorMessage, typeof(GeolocationDatabaseRegistrationExtensions), nameof(AddGeolocationDatabase));

                return;
            }

            options.UseSqlServer(connectionString, opts =>
            {
                opts.MigrationsHistoryTable("MigrationHistory_Geolocation");
            });
        });

        services.AddDatabaseDeveloperPageExceptionFilter();
        services.RemoveAll<IGeolocationRepository>();
        services.AddScoped<IGeolocationRepository, GeolocationRepository>();

        return services;
    }

    /// <summary>
    /// Applies pending migrations to the database.
    /// </summary>
    /// <param name="app">The web application instance.</param>
    /// <returns>The configured web application instance.</returns>
    public static async Task<WebApplication> UseGeolocationDatabase(this WebApplication app)
    {
        var isRunningAutomatedTest = app.ParseBoolEnvironmentVariable("IS_RUNNING_AUTOMATED_TEST");

        if (app.Environment.EnvironmentName != "IntegrationTests" && !isRunningAutomatedTest)
        {
            using var scope = app.Services.CreateScope();
            var email = scope.ServiceProvider.GetRequiredService<IEmailService>();

            try
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<GeolocationDbContext>();
                await dbContext.Database.MigrateAsync(app.Lifetime.ApplicationStopping);
            }
            catch (Exception ex)
            {
                app.CreateLogger().LogError(ex.ToString());
                _ = email.SendErrorAsync(ex.ToString());

                return app;
            }
        }

        return app;
    }
}