using InvestmentPortfolio.Database.Investment;
using InvestmentPortfolio.Exceptions;
using InvestmentPortfolio.Repositories.Entities;
using Microsoft.EntityFrameworkCore;

namespace InvestmentPortfolio.Repositories.Investment;

/// <summary>
/// Represents a repository for managing investment entities in the database.
/// Implements the <see cref="IInvestmentRepository"/> interface.
/// </summary>
/// <param name="dbContext">The database context for investment entities.</param>
internal sealed class InvestmentRepository(InvestmentDbContext dbContext) : IInvestmentRepository
{    
    public async Task<IEnumerable<InvestmentEntity>> GetAllAsync(CancellationToken cancellationToken)
    {
        return await dbContext.Investments
            .AsNoTracking()
            .ToListAsync(cancellationToken);
    }
        
    public async Task<InvestmentEntity> GetByIdAsync(int id, CancellationToken cancellationToken)
    {
        return await dbContext.Investments
            .FindAsync(id, cancellationToken) 
            ?? throw new EntityNotFoundException(nameof(InvestmentEntity));
    }
        
    public async Task CreateAsync(InvestmentEntity? entity, CancellationToken cancellationToken)
    {        
        ArgumentNullException.ThrowIfNull(entity);

        dbContext.Investments.Add(entity);
        await dbContext.SaveChangesAsync(cancellationToken);
    }
        
    public async Task UpdateAsync(InvestmentEntity? entity, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(entity);

        var investment = await GetByIdAsync(entity.Id, cancellationToken);

        investment.Name = entity.Name;
        investment.Value = entity.Value;
        investment.CurrencyCode = entity.CurrencyCode;
        await dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(int id, CancellationToken cancellationToken)
    {
        var investment = await GetByIdAsync(id, cancellationToken);

        dbContext.Investments.Remove(investment);
        await dbContext.SaveChangesAsync(cancellationToken);
    }
}