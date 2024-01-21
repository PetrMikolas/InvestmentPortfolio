using InvestmentPortfolio.Database.Investment;
using InvestmentPortfolio.Repositories.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace InvestmentPortfolio.UnitTests;

internal sealed class InMemoryDbContextWrapper
{
    public InvestmentDbContext DbContext { get; }

    public InMemoryDbContextWrapper()
    {
        var options = new DbContextOptionsBuilder<InvestmentDbContext>()
            .UseInMemoryDatabase("InvestmentTest")
            .ConfigureWarnings(b => b.Ignore(InMemoryEventId.TransactionIgnoredWarning))
            .Options;

        var context = new InvestmentDbContext(options);
        SeedInvestments(context);

        DbContext = context;
    }

    private void SeedInvestments(InvestmentDbContext dbContext)
    {
        dbContext.Database.EnsureDeleted();
        dbContext.Database.EnsureCreated();

        dbContext.Investments.AddRange(
        [
            new InvestmentEntity
            {
                Id = 1,
                Name = "Investice CZK",
                Value = 22500,
                CurrencyCode = "CZK"
            },
            new InvestmentEntity
            {
                Id = 2,
                Name = "Investice USD",
                Value = 1000,
                CurrencyCode = "USD"
            },
            new InvestmentEntity
            {
                Id = 3,
                Name = "Investice HUF",
                Value = 300000,
                CurrencyCode = "HUF"
            }
        ]);

        dbContext.SaveChanges();
    }
}