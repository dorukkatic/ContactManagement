using ContactReports.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ContactReports.DataAccess.Configurations;

public class ReportsConfiguration :  IEntityTypeConfiguration<Report>
{
    public void Configure(EntityTypeBuilder<Report> builder)
    {
        builder.ToTable("Reports");

        builder.HasKey(r => r.Id);

        builder.Property(r => r.Type)
            .IsRequired();
        
        builder.Property(r => r.Data)
            .HasColumnType("jsonb");

        builder.HasMany(r => r.Statuses)
            .WithOne()
            .HasForeignKey(rs => rs.ReportId);
    }
}