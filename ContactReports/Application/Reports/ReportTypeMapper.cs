using ContactReports.Contracts;
using ContactReports.Domain;
using Riok.Mapperly.Abstractions;

namespace ContactReports.Application.Reports;

[Mapper]
public partial class ReportTypeMapper
{
    public partial ReportType MapReportTypeDtoToReportType(ReportTypeDto reportTypeDto);
}