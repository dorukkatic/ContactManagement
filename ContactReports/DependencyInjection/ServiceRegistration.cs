using ContactReports.Application.Abstractions;
using ContactReports.Application.Reports;
using ContactReports.Application.Reports.Configurations;
using ContactReports.Contracts;
using ContactReports.DataAccess;
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
        PeopleServiceClientConfig peopleServiceClientConfig)
    {
        services.AddDbContext<ContactReportsDbContext>(options =>
            options.UseNpgsql(connectionString));
        
        services.AddHttpClient(peopleServiceClientConfig.ClientName, client =>
        {
            client.BaseAddress = new Uri(peopleServiceClientConfig.BaseUrl);
        });
        
        services.Configure<PeopleServiceClientConfig>(options =>
        {
            options.ClientName = peopleServiceClientConfig.ClientName;
            options.BaseUrl = peopleServiceClientConfig.BaseUrl;
            options.PeopleStatisticsEndpoint = peopleServiceClientConfig.PeopleStatisticsEndpoint;
        });
        
        services.AddSingleton(TimeProvider.System);

        services.AddMassTransit(messagingSettings);

        services.AddTransient<IEventBus, EventBus>();

        services.TryAddScoped<IReportService, ReportService>();
        services.TryAddScoped<IInternalReportService, ReportService>();


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