using Contacts.DataAccess;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Contacts.DependencyInjection;

public static class ServiceRegistration
{
    public static IServiceCollection AddContactsServices(
        this IServiceCollection services, 
        string connectionString)
    {
        services.AddDbContext<ContactsDbContext>(options =>
            options.UseNpgsql(connectionString));

        return services;
    }
}