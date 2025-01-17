﻿using ContactReports.Application.Reports.Configurations;
using ContactReports.Domain;
using FluentResults;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace ContactReports.Application.Reports;

public class PeopleByLocationReportGenerator : IReportGenerator
{
    private readonly HttpClient peopleStatisticsClient;
    private readonly PeopleServiceClientConfig peopleServiceClientConfig;
    private const ReportType GeneratorReportType = ReportType.PeopleByLocation;

    public PeopleByLocationReportGenerator(
        IHttpClientFactory httpClientFactory,
        IOptions<PeopleServiceClientConfig> peopleServiceClientConfig)
    {
        this.peopleServiceClientConfig = peopleServiceClientConfig.Value;
        peopleStatisticsClient = httpClientFactory.CreateClient(this.peopleServiceClientConfig.ClientName);
    }

    public bool CanGenerate(ReportType reportType)
    {
        return GeneratorReportType == reportType;
    }

    public async Task<Result<object>> GenerateReport(CancellationToken cancellationToken = default)
    {
        try
        {
            var result = 
                await peopleStatisticsClient.GetAsync(
                    peopleServiceClientConfig.PeopleStatisticsEndpoint, 
                    cancellationToken);
            
            if (!result.IsSuccessStatusCode) 
            {
                return Result.Fail("Failed to fetch people statistics");
            }
            
            var content = await result.Content.ReadAsStringAsync(cancellationToken);

            var returnType = GeneratorReportType.GetReportObjectType();
            var data = JsonConvert.DeserializeObject(content, returnType);
            return 
                data == null 
                    ? Result.Fail("Failed to deserialize people statistics") 
                    : Result.Ok(data);
        }
        catch (Exception e)
        {
            return Result.Fail(new Error("Failed to fetch people statistics").CausedBy(e));
        }
    }
}