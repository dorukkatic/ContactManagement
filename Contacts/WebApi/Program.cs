using Contacts.DependencyInjection;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
var connectionString = builder.Configuration.GetConnectionString("ContactsDb");

if(connectionString is null)
{
    throw new InvalidOperationException("Connection string 'ContactsDb' not found.");
}

builder.Services.AddContactsServices(connectionString);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Contacts API", Version = "v1" });
    c.SwaggerDoc("Person", new OpenApiInfo { Title = "Person API", Version = "v1" });
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Contacts API V1");
        c.SwaggerEndpoint("/swagger/Person/swagger.json", "Person API V1");
    });
}

app.UseRouting();
app.UseHttpsRedirection();
app.MapControllers();

app.Run();
