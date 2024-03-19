using InvestmentPortfolio.Database.Investment;
using InvestmentPortfolio.Helpers;
using InvestmentPortfolio.Repositories;
using InvestmentPortfolio.Repositories.Investment;
using InvestmentPortfolio.Services.Email;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace InvestmentPortfolio.Database;

public static class DatabaseInvestmentRegistrationExtensions
{
    public static IServiceCollection AddDatabaseInvestment(this IServiceCollection services, IConfiguration configuration, IEmailService email)
    {
        var connectionString = configuration.GetConnectionString("Investment");

        if (string.IsNullOrEmpty(connectionString))
        {            
            string errorMessage = "Nelze získat connection string na připojení databáze Investment";
            Type classType = typeof(DatabaseInvestmentRegistrationExtensions);

            email.SendError(errorMessage, classType, nameof(AddDatabaseInvestment));
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
                email.SendError(ex.ToString());
                app.Logger.LogError(ex.ToString());

                return app;
            }
        }

        return app;
    }
}