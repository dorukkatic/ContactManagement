using Bogus;
using Bogus.DataSets;
using ContactReports.DataAccess;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Time.Testing;

namespace ContactReports.Application.Tests.Unit;

public abstract class ReportsTestBase : IDisposable
{
    protected readonly ContactReportsDbContext db;
    protected readonly Date DateFaker = new Faker().Date;
    protected readonly FakeTimeProvider ContactsManagementTimeProvider = new();
    
    protected ReportsTestBase()
    {
        
        var connection = new SqliteConnection("DataSource=:memory:");
        connection.Open();

        var options = 
            new DbContextOptionsBuilder<ContactReportsDbContext>()
                .UseSqlite(connection)
                .Options;

        db = new ContactReportsDbContext(options, ContactsManagementTimeProvider);
        db.Database.EnsureCreated();
    }

    protected virtual Task SeedDatabaseAsync() => Task.CompletedTask;

    public void Dispose()
    {
        db.Dispose();
    }
}