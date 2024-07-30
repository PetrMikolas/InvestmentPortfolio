using InvestmentPortfolio.Repositories.Entities;

namespace InvestmentPortfolio.Repositories.Geolocation;

/// <summary>
/// Represents a repository interface for managing geolocation entities.
/// </summary>
public interface IGeolocationRepository
{
    /// <summary>
    /// Retrieves all geolocation entities asynchronously.
    /// </summary>
    /// <param name="cancellationToken">The token to monitor for cancellation requests.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains an enumerable collection of geolocation entities.</returns>
    Task<IEnumerable<GeolocationEntity>> GetAllAsync(CancellationToken cancellationToken);

    /// <summary>
    /// Creates a new geolocation entity asynchronously.
    /// </summary>
    /// <param name="entity">The geolocation entity to create.</param>
    /// <param name="cancellationToken">The token to monitor for cancellation requests.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    /// <exception cref="ArgumentNullException">Thrown when the provided geolocation entity is null.</exception>
    Task CreateAsync(GeolocationEntity entity, CancellationToken cancellationToken);
}