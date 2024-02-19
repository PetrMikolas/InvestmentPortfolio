using InvestmentPortfolio.Database.Investment;
using InvestmentPortfolio.Repositories.Entities;

namespace InvestmentPortfolio.IntegrationTests;

internal static class Utilities
{
    internal static void InitializeDbForTesting(InvestmentDbContext dbContext)
    {
        dbContext.Database.EnsureDeleted();
        dbContext.Database.EnsureCreated();

        dbContext.Investments.AddRange(
        [
            new InvestmentEntity
            {
                Id = 1,
                Name = "Investice CZK",
                Value = 10000,
                CurrencyCode = "CZK"
            },
            new InvestmentEntity
            {
                Id = 2,
                Name = "Investice USD",
                Value = 1000,
                CurrencyCode = "USD",
                DefaultValueCzk = 22500
            },
            new InvestmentEntity
            {
                Id = 3,
                Name = "Investice EUR",
                Value = 1000,
                CurrencyCode = "EUR",
                DefaultValueCzk = 24500
            }
        ]);

        dbContext.SaveChanges();
    }
}