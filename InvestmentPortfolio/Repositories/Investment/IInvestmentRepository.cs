using InvestmentPortfolio.Exceptions;
using InvestmentPortfolio.Repositories.Entities;

namespace InvestmentPortfolio.Repositories;

/// <summary>
/// Represents a repository for managing investment entities in the database.
/// </summary>
public interface IInvestmentRepository
{
    /// <summary>
    /// Retrieves all investment entities asynchronously.
    /// </summary>
    /// <param name="cancellationToken">The token to monitor for cancellation requests.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains an enumerable collection of investment entities.</returns>
    Task<IEnumerable<InvestmentEntity>> GetAllAsync(CancellationToken cancellationToken);

    /// <summary>
    /// Retrieves an investment entity by its identifier asynchronously.
    /// </summary>
    /// <param name="id">The identifier of the investment entity to retrieve.</param>
    /// <param name="cancellationToken">The token to monitor for cancellation requests.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the investment entity.</returns>
    /// <exception cref="EntityNotFoundException">Thrown when the investment entity with the specified ID is not found.</exception>
    Task<InvestmentEntity> GetByIdAsync(int id, CancellationToken cancellationToken);

    /// <summary>
    /// Creates a new investment entity asynchronously.
    /// </summary>
    /// <param name="entity">The investment entity to create.</param>
    /// <param name="cancellationToken">The token to monitor for cancellation requests.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    /// <exception cref="ArgumentNullException">Thrown when the provided investment entity is null.</exception>
    Task CreateAsync(InvestmentEntity? entity, CancellationToken cancellationToken);

    /// <summary>
    /// Updates an existing investment entity asynchronously.
    /// </summary>
    /// <param name="entity">The updated investment entity.</param>
    /// <param name="cancellationToken">The token to monitor for cancellation requests.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    /// <exception cref="ArgumentNullException">Thrown when the provided investment entity is null.</exception>
    /// <exception cref="EntityNotFoundException">Thrown when the investment entity with the specified ID is not found.</exception>
    Task UpdateAsync(InvestmentEntity? entity, CancellationToken cancellationToken);

    /// <summary>
    /// Deletes an investment entity by its identifier asynchronously.
    /// </summary>
    /// <param name="id">The identifier of the investment entity to delete.</param>
    /// <param name="cancellationToken">The token to monitor for cancellation requests.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    /// <exception cref="EntityNotFoundException">Thrown when the investment entity with the specified ID is not found.</exception>
    Task DeleteAsync(int id, CancellationToken cancellationToken);
}