using InvestmentPortfolio.Repositories.Entities;
using Microsoft.EntityFrameworkCore;

namespace InvestmentPortfolio.Database.Investment;

/// <summary>
/// Represents the database context for investments.
/// </summary>
internal sealed class InvestmentDbContext : DbContext
{
    public InvestmentDbContext(DbContextOptions<InvestmentDbContext> options) : base(options) { }

    public DbSet<InvestmentEntity> Investments { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<InvestmentEntity>(entity =>
        {
            entity.HasKey(p => p.Id);
            entity.Property(p => p.Name).IsRequired().HasMaxLength(60);
            entity.Property(p => p.Value).IsRequired();
            entity.Property(p => p.CurrencyCode).IsRequired().HasMaxLength(3);
            entity.Property(p => p.CreatedDate).IsRequired();
            entity.Property(p => p.ModifiedDate).IsRequired(false);
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
                ((InvestmentEntity)entry.Entity).CreatedDate = DateTimeOffset.UtcNow;
                break;
            case EntityState.Modified:
                ((InvestmentEntity)entry.Entity).ModifiedDate = DateTimeOffset.UtcNow;
                break;
        }
    }
}