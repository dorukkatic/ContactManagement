using ContactReports.DataAccess;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace ContactReports.DependencyInjection;

public static class ServiceRegistration
{
    public static IServiceCollection AddContactReportsServices(
        this IServiceCollection services, 
        string connectionString)
    {
        services.AddDbContext<ContactReportsDbContext>(options =>
            options.UseNpgsql(connectionString));
        
        services.AddSingleton(TimeProvider.System);

        return services;
    }
}