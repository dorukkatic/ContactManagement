using ContactReports.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ContactReports.DataAccess.Configurations;

public class ReportStatusesConfiguration : IEntityTypeConfiguration<ReportStatus>
{
    public void Configure(EntityTypeBuilder<ReportStatus> builder)
    {
        builder.ToTable("ReportStatuses");

        builder.HasKey(rs => rs.Id);

        builder.Property(rs => rs.Status)
            .IsRequired();

        builder.Property(rs => rs.IsEnabled)
            .IsRequired();

        builder.HasOne(rs => rs.Report)
            .WithMany(r => r.Statuses)
            .HasForeignKey(rs => rs.ReportId);
    }
}