using System.Reflection;
using Contacts.DependencyInjection;
using Microsoft.OpenApi.Models;
using SharpGrip.FluentValidation.AutoValidation.Mvc.Extensions;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
var connectionString = builder.Configuration.GetConnectionString("ContactsDb");

if(connectionString is null)
{
    throw new InvalidOperationException("Connection string 'ContactsDb' not found.");
}

builder.Services.AddFluentValidationAutoValidation(configuration =>
{
    configuration.DisableBuiltInModelValidation = true;
    configuration.EnablePathBindingSourceAutomaticValidation = true;
});

builder.Services.AddContactsServices(connectionString);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    var version = Assembly
        .GetExecutingAssembly()
        .GetName()
        .Version?
        .ToString() ?? "1.0.0";

    c.SwaggerDoc("People", new OpenApiInfo
    {
        Title = "People API",
        Version = version
    });
    c.SwaggerDoc("Statistics", new OpenApiInfo
    {
        Title = "Statistics API",
        Version = version
    });
    
    // Set the comments path for the Swagger JSON and UI.
    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    c.IncludeXmlComments(xmlPath);
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.Services.RunDbMigrations();
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/People/swagger.json", "People API");
        c.SwaggerEndpoint("/swagger/Statistics/swagger.json", "Statistics API");
    });
}

app.UseRouting();
app.UseHttpsRedirection();
app.MapControllers();

app.Run();
