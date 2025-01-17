namespace ContactReports.Contracts;

public record ReportResponse(
    Guid Id,
    ReportTypeDto ReportType,
    StatusDto Status,
    DateTime RequestedAt,
    DateTime? CreatedAt);