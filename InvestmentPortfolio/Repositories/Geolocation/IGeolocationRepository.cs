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
    /// <returns>A task that represents the asynchronous operation. The task result contains a list of geolocation entities.</returns>
    Task<List<GeolocationEntity>> GetAllAsync(CancellationToken cancellationToken);

    /// <summary>
    /// Creates a new geolocation entity asynchronously.
    /// </summary>
    /// <param name="entity">The geolocation entity to create.</param>
    /// <param name="cancellationToken">The token to monitor for cancellation requests.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    Task CreateAsync(GeolocationEntity entity, CancellationToken cancellationToken);
}