using InvestmentPortfolio.Database.Geolocation;
using InvestmentPortfolio.Repositories.Entities;
using Microsoft.EntityFrameworkCore;

namespace InvestmentPortfolio.Repositories.Geolocation;

/// <summary>
/// Represents a repository for managing geolocation entities in the database.
/// Implements the <see cref="IGeolocationRepository"/> interface.
/// </summary>
/// <param name="dbContext">The database context for geolocation entities.</param>
internal sealed class GeolocationRepository(GeolocationDbContext dbContext) : IGeolocationRepository
{    
    public async Task<List<GeolocationEntity>> GetAllAsync(CancellationToken cancellationToken)
    {
        return await dbContext.Geolocations.AsNoTracking().OrderByDescending(e => e.Id).ToListAsync(cancellationToken);
    }
    
    public async Task CreateAsync(GeolocationEntity entity, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(entity);

        dbContext.Geolocations.Add(entity);
        await dbContext.SaveChangesAsync(cancellationToken);
    }
}