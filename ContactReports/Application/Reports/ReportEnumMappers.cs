using ContactReports.Contracts;
using ContactReports.Domain;
using Riok.Mapperly.Abstractions;

namespace ContactReports.Application.Reports;

[Mapper]
public partial class ReportTypeMapper
{
    public partial ReportType MapReportTypeDtoToReportType(ReportTypeDto reportTypeDto);
    public partial ReportTypeDto MapReportTypeToReportTypeDto(ReportType reportType);
}

[Mapper]
public partial class ReportStatusMapper
{
    public partial StatusDto MapStatusDtoToReportStatus(Status reportStatus);
}