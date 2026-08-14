using System;
using System.Threading;
using System.Threading.Tasks;
using Limbo.Umbraco.Fetch.Services;
using Microsoft.Extensions.Logging;
using Umbraco.Cms.Infrastructure.HostedServices;

#pragma warning disable CS1591

namespace Limbo.Umbraco.Fetch.Scheduling;

public class FetchTask : RecurringHostedServiceBase {

    private readonly FetchService _fetchService;

    private static TimeSpan Period => TimeSpan.FromMinutes(1);

    private static TimeSpan Delay => TimeSpan.FromMinutes(1);

    public FetchTask(ILogger<FetchTask> logger, FetchService fetchService, TimeProvider timeProvider) : base(logger, Period, Delay, timeProvider) {
        _fetchService = fetchService;
    }

    public override async Task PerformExecuteAsync(CancellationToken stoppingToken) {
        if (stoppingToken.IsCancellationRequested) return;
        await _fetchService.FetchAll();
    }

}