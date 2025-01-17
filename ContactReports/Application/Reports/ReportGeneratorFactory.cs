using ContactReports.Domain;

namespace ContactReports.Application.Reports;

public class ReportGeneratorFactory : IReportGeneratorFactory
{
    private readonly List<IReportGenerator> reportGenerators;

    public ReportGeneratorFactory(IEnumerable<IReportGenerator> reportGenerators)
    {
        this.reportGenerators = reportGenerators.ToList();
    }
    
    public IReportGenerator GetReportGenerator(ReportType reportType)
    {
        var reportGenerator = reportGenerators.FirstOrDefault(rg => rg.CanGenerate(reportType));
        
        if (reportGenerator == null)
        {
            throw new InvalidOperationException($"No report generator found for report type {reportType}");
        }
        
        return reportGenerator;
    }
}