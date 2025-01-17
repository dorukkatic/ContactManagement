using ContactReports.Application.Reports.Configurations;
using ContactReports.Domain;
using FluentResults;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace ContactReports.Application.Reports;

public class PeopleByLocationReportGenerator : IReportGenerator
{
    private readonly HttpClient peopleStatisticsClient;
    private readonly PeopleServiceClientConfig peopleServiceClientConfig;

    public PeopleByLocationReportGenerator(
        IHttpClientFactory httpClientFactory,
        IOptions<PeopleServiceClientConfig> peopleServiceClientConfig)
    {
        this.peopleServiceClientConfig = peopleServiceClientConfig.Value;
        peopleStatisticsClient = httpClientFactory.CreateClient(this.peopleServiceClientConfig.ClientName);
    }

    public bool CanGenerate(ReportType reportType)
    {
        return reportType == ReportType.PeopleByLocation;
    }

    public async Task<Result<T>> GenerateReport<T>(CancellationToken cancellationToken = default)
    {
        var result = 
            await peopleStatisticsClient.GetAsync(
                peopleServiceClientConfig.PeopleStatisticsEndpoint, 
                cancellationToken);
        
        if (!result.IsSuccessStatusCode) 
        {
            return Result.Fail<T>("Failed to fetch people statistics");
        }
        
        var content = await result.Content.ReadAsStringAsync(cancellationToken);

        try
        {
            var data = JsonConvert.DeserializeObject<T>(content);
            return 
                data == null 
                    ? Result.Fail("Failed to deserialize people statistics") 
                    : Result.Ok(data);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }
}