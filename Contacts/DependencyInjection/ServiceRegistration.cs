using Contacts.Application;
using Contacts.Application.ContactInfos;
using Contacts.Application.People;
using Contacts.Application.Statistics;
using Contacts.Contracts;
using Contacts.Contracts.ContactInfos;
using Contacts.Contracts.People;
using Contacts.Contracts.Statistics;
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
        services.AddScoped<IContactInfosService, ContactInfosService>();
        services.AddScoped<IStatisticsService, StatisticsService>();
        
        services.AddFluentValidation();
        services.AddSingleton(TimeProvider.System);

        return services;
    }
    
    private static IServiceCollection AddFluentValidation(this IServiceCollection services)
    {
        services.AddValidatorsFromAssembly(typeof(AssemblyReference).Assembly);
        return services;
    }
}