using ContactReports.Application.Abstractions;
using ContactReports.Application.Reports;
using ContactReports.Application.Reports.Configurations;
using ContactReports.Contracts;
using ContactReports.DataAccess;
using FluentValidation;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace ContactReports.DependencyInjection;

public static class ServiceRegistration
{
    public static IServiceCollection AddContactReportsServices(
        this IServiceCollection services, 
        string connectionString,
        MessagingSettings messagingSettings,
        ContactsServiceClientConfig contactsServiceClientConfig)
    {
        services.AddDbContext<ContactReportsDbContext>(options =>
            options.UseNpgsql(connectionString));
        
        services.AddHttpClient(contactsServiceClientConfig.ClientName, client =>
        {
            client.BaseAddress = new Uri(contactsServiceClientConfig.BaseUrl);
        });
        
        services.Configure<ContactsServiceClientConfig>(options =>
        {
            options.ClientName = contactsServiceClientConfig.ClientName;
            options.BaseUrl = contactsServiceClientConfig.BaseUrl;
            options.PeopleStatisticsEndpoint = contactsServiceClientConfig.PeopleStatisticsEndpoint;
        });
        
        services.AddSingleton(TimeProvider.System);

        services.AddMassTransit(messagingSettings);
        services.AddFluentValidation();

        services.AddTransient<IEventBus, EventBus>();
        services.AddScoped<IReportGeneratorFactory, ReportGeneratorFactory>();
        services.AddTransient<IReportGenerator, PeopleByLocationReportGenerator>();

        services.TryAddScoped<IReportService, ReportService>();
        services.TryAddScoped<IInternalReportService, ReportService>();


        return services;
    }
    
    private static IServiceCollection AddFluentValidation(this IServiceCollection services)
    {
        services.AddValidatorsFromAssembly(typeof(AssemblyReference).Assembly);
        return services;
    }
    
    private static IServiceCollection AddMassTransit(this IServiceCollection services, MessagingSettings messagingSettings)
    {
        services.AddMassTransit(busConfigurator =>
        {
            busConfigurator.SetKebabCaseEndpointNameFormatter();

            busConfigurator.AddConsumer<ReportRequestedEventConsumer>();

            busConfigurator.UsingRabbitMq((context, configurator) =>
            {
                busConfigurator.SetKebabCaseEndpointNameFormatter();
                
                configurator.Host(new Uri(messagingSettings.Host), h =>
                {
                    h.Username(messagingSettings.UserName);
                    h.Password(messagingSettings.Password);
                });

                configurator.ConfigureEndpoints(context);
            });
        });

        return services;
    }
}