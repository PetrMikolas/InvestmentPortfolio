using InvestmentPortfolio.Models;
using InvestmentPortfolio.Repositories.Entities;

namespace InvestmentPortfolio.Services.Investment;

/// <summary>
/// Provides methods for managing investments.
/// </summary>
public interface IInvestmentService
{
    /// <summary>
    /// Retrieves all investments asynchronously.
    /// </summary>
    /// <param name="hasRefreshExchangeRates">Indicates whether to refresh exchange rates.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>Returns all investments.</returns>
    Task<Investments> GetAllAsync(bool hasRefreshExchangeRates, CancellationToken cancellationToken);

    /// <summary>
    /// Deletes an investment by ID asynchronously.
    /// </summary>
    /// <param name="id">The ID of the investment to delete.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    Task DeleteAsync(int id, CancellationToken cancellationToken);

    /// <summary>
    /// Creates a new investment asynchronously.
    /// </summary>
    /// <param name="entity">The investment entity to create.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    Task CreateAsync(InvestmentEntity? entity, CancellationToken cancellationToken);

    /// <summary>
    /// Updates an existing investment asynchronously.
    /// </summary>
    /// <param name="entity">The updated investment entity.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    Task UpdateAsync(InvestmentEntity? entity, CancellationToken cancellationToken);
}