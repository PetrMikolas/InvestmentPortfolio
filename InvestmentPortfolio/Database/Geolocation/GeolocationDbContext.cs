using InvestmentPortfolio.Repositories.Entities;
using Microsoft.EntityFrameworkCore;

namespace InvestmentPortfolio.Database.Geolocation;

internal sealed class GeolocationDbContext : DbContext
{
    public GeolocationDbContext(DbContextOptions<GeolocationDbContext> options) : base(options) { }

    public DbSet<GeolocationEntity> Geolocations => Set<GeolocationEntity>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<GeolocationEntity>(entity =>
        {
            entity.HasKey(p => p.Id);
            entity.Property(p => p.IpAddress).IsRequired().HasMaxLength(40);
            entity.Property(p => p.City).IsRequired().HasMaxLength(100);
            entity.Property(p => p.Country).IsRequired().HasMaxLength(50);
            entity.Property(p => p.Isp).IsRequired().HasMaxLength(100);
            entity.Property(p => p.LocalDate).IsRequired().HasMaxLength(20);
            entity.Property(p => p.Referer).IsRequired().HasMaxLength(150);
        });
    }
}