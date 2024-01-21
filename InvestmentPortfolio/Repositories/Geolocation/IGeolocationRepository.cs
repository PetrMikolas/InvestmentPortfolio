using InvestmentPortfolio.Repositories.Entities;

namespace InvestmentPortfolio.Repositories.Geolocation;

public interface IGeolocationRepository
{
    Task<List<GeolocationEntity>> GetAllAsync(CancellationToken cancellationToken);

    Task CreateAsync(GeolocationEntity entity, CancellationToken cancellationToken);
}