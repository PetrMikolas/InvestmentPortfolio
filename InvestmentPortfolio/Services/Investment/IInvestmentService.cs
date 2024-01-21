using InvestmentPortfolio.Models;
using InvestmentPortfolio.Repositories.Entities;

namespace InvestmentPortfolio.Services.Investment;

public interface IInvestmentService
{
    Task<Investments> GetAllAsync(bool hasRefreshExchangeRates, CancellationToken cancellationToken);

    Task DeleteAsync(int id, CancellationToken cancellationToken);

    Task CreateAsync(InvestmentEntity entity, CancellationToken cancellationToken);

    Task UpdateAsync(InvestmentEntity entity, CancellationToken cancellationToken);
}