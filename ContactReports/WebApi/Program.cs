using System.Reflection;
using ContactReports.Application.Reports.Configurations;
using ContactReports.DependencyInjection;
using Microsoft.OpenApi.Models;
using SharpGrip.FluentValidation.AutoValidation.Mvc.Extensions;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
var connectionString = builder.Configuration.GetConnectionString("ContactsDb");
var messagingSettings = builder.Configuration.GetSection("MessagingSettings").Get<MessagingSettings>();
var peopleServiceClientConfig = builder.Configuration.GetSection("PeopleServiceClientConfig").Get<PeopleServiceClientConfig>();

if(messagingSettings is null)
{
    throw new InvalidOperationException("Messaging settings not found.");
}
if(connectionString is null)
{
    throw new InvalidOperationException("Connection string 'ContactsDb' not found.");
}
if(peopleServiceClientConfig is null)
{
    throw new InvalidOperationException("People service client config not found.");
}

builder.Services.AddContactReportsServices(connectionString, messagingSettings, peopleServiceClientConfig);

builder.Services.AddFluentValidationAutoValidation(configuration =>
{
    configuration.DisableBuiltInModelValidation = true;
    configuration.EnablePathBindingSourceAutomaticValidation = true;
});



builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    var version = Assembly
        .GetExecutingAssembly()
        .GetName()
        .Version?
        .ToString() ?? "1.0.0";

    c.SwaggerDoc("Reports", new OpenApiInfo
    {
        Title = "Reports API",
        Version = version
    });
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.Services.RunDbMigrations();
    
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/Reports/swagger.json", "Reports API");
    });
}

app.UseRouting();
app.UseHttpsRedirection();
app.MapControllers();

app.Run();
