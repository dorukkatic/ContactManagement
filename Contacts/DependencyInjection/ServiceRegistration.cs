using Contacts.Application;
using Contacts.Application.Person;
using Contacts.Contracts;
using Contacts.Contracts.Person;
using Contacts.DataAccess;
using FluentValidation;
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
        
        services.AddScoped<IPeopleService, PeopleService>();
        services.AddFluentValidation();

        return services;
    }
    
    private static IServiceCollection AddFluentValidation(this IServiceCollection services)
    {
        services.AddValidatorsFromAssembly(typeof(AssemblyReference).Assembly);
        return services;
    }
}