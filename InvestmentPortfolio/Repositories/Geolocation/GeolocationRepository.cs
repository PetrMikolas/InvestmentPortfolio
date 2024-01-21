using InvestmentPortfolio.Database.Geolocation;
using InvestmentPortfolio.Repositories.Entities;
using Microsoft.EntityFrameworkCore;

namespace InvestmentPortfolio.Repositories.Geolocation;

internal class GeolocationRepository(GeolocationDbContext dbContext) : IGeolocationRepository
{
    public async Task<List<GeolocationEntity>> GetAllAsync(CancellationToken cancellationToken)
    {
        return await dbContext.Geolocations.AsNoTracking().ToListAsync(cancellationToken);
    }

    public async Task CreateAsync(GeolocationEntity entity, CancellationToken cancellationToken)
    {        
        dbContext.Geolocations.Add(entity);
        await dbContext.SaveChangesAsync(cancellationToken);
    }
}