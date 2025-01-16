using Contacts.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Contacts.DataAccess.Configurations;

public class PeopleConfiguration : IEntityTypeConfiguration<Person>
{
    public void Configure(EntityTypeBuilder<Person> builder)
    {
        builder.ToTable("People");
        
        builder.HasMany(p => p.ContactInfos)
            .WithOne(ci => ci.Person)
            .HasForeignKey(ci => ci.PersonId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}