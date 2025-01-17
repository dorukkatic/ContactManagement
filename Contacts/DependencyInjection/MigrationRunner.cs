using Contacts.DataAccess;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Contacts.DependencyInjection;

public static class MigrationRunner
{
    public static void RunDbMigrations(this IServiceProvider serviceProvider)
    {
        using var scope = serviceProvider.CreateScope();
        var services = scope.ServiceProvider;
        
        var context = services.GetRequiredService<ContactsDbContext>();
        context.Database.Migrate(); // Applies pending migrations
    }
}