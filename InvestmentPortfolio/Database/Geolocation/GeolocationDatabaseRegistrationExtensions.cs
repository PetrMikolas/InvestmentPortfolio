﻿using InvestmentPortfolio.Helpers;
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
    private static readonly ILogger _logger = LoggerFactory
        .Create(builder => builder.AddConsole().AddDebug())
        .CreateLogger(typeof(GeolocationDatabaseRegistrationExtensions));

    /// <summary>
    /// Adds database services related to geolocation to the specified <see cref="IServiceCollection"/>.
    /// </summary>
    /// <param name="services">The collection of services in the application.</param>
    /// <param name="configuration">The configuration of the application.</param>
    /// <param name="email">The service for sending email notifications.</param>
    /// <returns>The collection of services with added database services.</returns>
    public static IServiceCollection AddGeolocationDatabase(this IServiceCollection services, IConfiguration configuration, IEmailService email)
    {
        var connectionString = configuration.GetConnectionString("Geolocation");

        if (string.IsNullOrWhiteSpace(connectionString))
        {
            string errorMessage = "Nelze získat connection string na připojení databáze Geolocation";           

            _ = email.SendErrorAsync(errorMessage, typeof(GeolocationDatabaseRegistrationExtensions), nameof(AddGeolocationDatabase));
            _logger.LogError(errorMessage);

            return services;
        }

        services.AddDbContext<GeolocationDbContext>(options =>
        {
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
    /// <param name="email">The service for sending email notifications.</param>
    /// <returns>The configured web application instance.</returns>
    public static WebApplication UseGeolocationDatabase(this WebApplication app, IEmailService email)
    {
        var isRunningAutomatedTest = Helper.ParseBoolEnvironmentVariable("IS_RUNNING_AUTOMATED_TEST");

        if (app.Environment.EnvironmentName != "IntegrationTests" && !isRunningAutomatedTest)
        {
            try
            {
                using var scope = app.Services.CreateScope();
                var dbContext = scope.ServiceProvider.GetRequiredService<GeolocationDbContext>();
                dbContext.Database.Migrate();
            }
            catch (Exception ex)
            {
                _ = email.SendErrorAsync(ex.ToString());
                _logger.LogError(ex.ToString());

                return app;
            }
        }

        return app;
    }
}