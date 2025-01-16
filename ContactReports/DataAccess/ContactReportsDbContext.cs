using System.Reflection;
using ContactReports.Domain;
using Microsoft.EntityFrameworkCore;

namespace ContactReports.DataAccess;

public class ContactReportsDbContext : DbContext
{
    private const string Schema = "contactReports";
    
    private readonly TimeProvider timeProvider;

    public ContactReportsDbContext(DbContextOptions<ContactReportsDbContext> options, TimeProvider timeProvider)
        : base(options)
    {
        this.timeProvider = timeProvider;
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema(Schema);
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        base.OnModelCreating(modelBuilder);
    }
    
    public override int SaveChanges()
    {
        UpdateTimestamps();
        return base.SaveChanges();
    }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        UpdateTimestamps();
        return await base.SaveChangesAsync(cancellationToken);
    }

    private void UpdateTimestamps()
    {
        var entities =
            ChangeTracker.Entries<BaseEntity>()
                .Where(e => e.State is EntityState.Added or EntityState.Modified);

        foreach (var entity in entities)
        {
            if (entity.State == EntityState.Added)
            {
                entity.Entity.SetCreationTimestamp(timeProvider);
            }

            if (entity.State == EntityState.Modified)
            {
                entity.Entity.SetModificationTimestamp(timeProvider);
            }
        }
    }

}