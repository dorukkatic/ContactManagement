using System.Text.Json.Nodes;

namespace ContactReports.Contracts;

public record ReportDetailResponse(
    Guid Id,
    ReportTypeDto ReportType,
    StatusDto Status,
    DateTime RequestedAt,
    DateTime? GeneratedAt,
    dynamic? Data);