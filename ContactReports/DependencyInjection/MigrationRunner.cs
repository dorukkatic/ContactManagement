using ContactReports.DataAccess;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace ContactReports.DependencyInjection;

public static class MigrationRunner
{
    public static void RunDbMigrations(this IServiceProvider serviceProvider)
    {
        using var scope = serviceProvider.CreateScope();
        var services = scope.ServiceProvider;
        
        var context = services.GetRequiredService<ContactReportsDbContext>();
        context.Database.Migrate(); // Applies pending migrations
    }
}