using InvestmentPortfolio.Repositories.Entities;
using Microsoft.EntityFrameworkCore;

namespace InvestmentPortfolio.Database.Investment;

/// <summary>
/// Represents the database context for investments.
/// </summary>
internal sealed class InvestmentDbContext(DbContextOptions<InvestmentDbContext> options) : DbContext(options)
{
    public DbSet<InvestmentEntity> Investments { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<InvestmentEntity>(entity =>
        {
            entity.HasKey(p => p.Id);
            entity.Property(p => p.Name).IsRequired().HasMaxLength(60);
            entity.Property(p => p.Value).IsRequired();
            entity.Property(p => p.CurrencyCode).IsRequired().HasMaxLength(3);
            entity.Property(p => p.CreatedAt).IsRequired();
            entity.Property(p => p.UpdatedAt).IsRequired(false);
            entity.Property(p => p.DefaultValueCzk).IsRequired(false);
        });
    }

    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        AddTimestamps();
        return base.SaveChangesAsync(cancellationToken);
    }

    private void AddTimestamps()
    {
        var entry = ChangeTracker.Entries().First();

        switch (entry.State)
        {
            case EntityState.Added:
                ((InvestmentEntity)entry.Entity).CreatedAt = DateTimeOffset.UtcNow;
                break;
            case EntityState.Modified:
                ((InvestmentEntity)entry.Entity).UpdatedAt = DateTimeOffset.UtcNow;
                break;
        }
    }
}