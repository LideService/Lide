using Lide.Demo.Exchange.DAL.Model;
using Microsoft.EntityFrameworkCore;

namespace Lide.Demo.Exchange.DAL.Service;

public class ExchangeContext : DbContext
{
    public ExchangeContext(DbContextOptions<ExchangeContext> options)
        : base(options)
    {
    }

    public DbSet<Currency> Currencies { get; set; } = null!;
    public DbSet<ExchangeData> ExchangeRates { get; set; } = null!;

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        ChangeTracker.AutoDetectChangesEnabled = false;
        ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;
        optionsBuilder.UseInMemoryDatabase(databaseName: "AuthorDb");
    }
}