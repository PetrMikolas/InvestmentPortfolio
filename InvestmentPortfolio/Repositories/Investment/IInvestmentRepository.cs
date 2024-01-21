using InvestmentPortfolio.Repositories.Entities;

namespace InvestmentPortfolio.Repositories;

public interface IInvestmentRepository
{
    Task<List<InvestmentEntity>> GetAllAsync(CancellationToken cancellationToken);

    Task<InvestmentEntity> GetByIdAsync(int id, CancellationToken cancellationToken);

    Task CreateAsync(InvestmentEntity entity, CancellationToken cancellationToken);

    Task UpdateAsync(InvestmentEntity entity, CancellationToken cancellationToken);

    Task DeleteAsync(int id, CancellationToken cancellationToken);
}