using InvestmentPortfolio.Database.Investment;
using InvestmentPortfolio.Exceptions;
using InvestmentPortfolio.Repositories.Entities;
using Microsoft.EntityFrameworkCore;

namespace InvestmentPortfolio.Repositories.Investment;

internal sealed class InvestmentRepository(InvestmentDbContext dbContext) : IInvestmentRepository
{
    public async Task<List<InvestmentEntity>> GetAllAsync(CancellationToken cancellationToken)
    {
        return await dbContext.Investments.AsNoTracking().ToListAsync(cancellationToken);
    }

    public async Task<InvestmentEntity> GetByIdAsync(int id, CancellationToken cancellationToken)
    {
        return await dbContext.Investments.FindAsync(id, cancellationToken) ?? throw new EntityNotFoundException(nameof(InvestmentEntity));
    }

    public async Task CreateAsync(InvestmentEntity? entity, CancellationToken cancellationToken)
    {
        _ = entity ?? throw new ArgumentNullException(nameof(entity));

        dbContext.Investments.Add(entity);
        await dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateAsync(InvestmentEntity? entity, CancellationToken cancellationToken)
    {
        _ = entity ?? throw new ArgumentNullException(nameof(entity));

        var investment = await dbContext.Investments.FindAsync(entity.Id, cancellationToken) ?? throw new EntityNotFoundException(nameof(InvestmentEntity));
        investment.Name = entity.Name;
        investment.Value = entity.Value;
        investment.CurrencyCode = entity.CurrencyCode;
        await dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(int id, CancellationToken cancellationToken)
    {
        var investment = await dbContext.Investments.FindAsync(id, cancellationToken) ?? throw new EntityNotFoundException(nameof(InvestmentEntity));
        dbContext.Investments.Remove(investment);
        await dbContext.SaveChangesAsync(cancellationToken);
    }
}