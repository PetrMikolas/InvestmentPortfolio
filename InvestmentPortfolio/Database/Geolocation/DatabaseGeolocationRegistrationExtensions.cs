﻿using InvestmentPortfolio.Repositories.Geolocation;
using InvestmentPortfolio.Services.Email;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace InvestmentPortfolio.Database.Geolocation;

public static class DatabaseGeolocationRegistrationExtensions
{
    public static IServiceCollection AddDatabaseGeolocation(this IServiceCollection services, IConfiguration configuration, IEmailService email)
    {
        var connectionString = configuration.GetConnectionString("Geolocation");

        if (string.IsNullOrEmpty(connectionString))            
        {
            string errorMessage = "Nelze získat connection string na připojení databáze Geolocation";
            Type classType = typeof(DatabaseGeolocationRegistrationExtensions);

            email.SendError(errorMessage, classType, nameof(AddDatabaseGeolocation));
            LoggerFactory.Create(builder => builder.AddConsole()).CreateLogger(classType).LogError(errorMessage);

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

    public static WebApplication UseDatabaseGeolocation(this WebApplication app, IEmailService email)
    {
        if (app.Environment.EnvironmentName != "IntegrationTests")
        {
            try
            {
                using var scope = app.Services.CreateScope();
                var dbContext = scope.ServiceProvider.GetRequiredService<GeolocationDbContext>();                
                dbContext.Database.Migrate();
            }
            catch (Exception ex)
            {
                email.SendError(ex.ToString());
                app.Logger.LogError(ex.ToString());

                return app;
            }
        }

        return app;
    }
}