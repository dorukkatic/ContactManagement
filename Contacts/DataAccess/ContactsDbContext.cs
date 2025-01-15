using System.Reflection;
using Contacts.Domain;
using Microsoft.EntityFrameworkCore;

namespace Contacts.DataAccess;
public class ContactsDbContext : DbContext
{
    public DbSet<Person> People { get; set; }

    public ContactsDbContext(DbContextOptions<ContactsDbContext> options)
        : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        base.OnModelCreating(modelBuilder);
    }
}
