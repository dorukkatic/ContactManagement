using System.Reflection;
using ContactReports.DependencyInjection;
using Microsoft.OpenApi.Models;
using SharpGrip.FluentValidation.AutoValidation.Mvc.Extensions;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
var connectionString = builder.Configuration.GetConnectionString("ContactsDb");
var messagingSettings = builder.Configuration.GetSection("MessagingSettings").Get<MessagingSettings>();

if(messagingSettings is null)
{
    throw new InvalidOperationException("Messaging settings not found.");
}

if(connectionString is null)
{
    throw new InvalidOperationException("Connection string 'ContactsDb' not found.");
}

builder.Services.AddContactReportsServices(connectionString, messagingSettings);

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
