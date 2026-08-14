using System;
using System.Threading;
using System.Threading.Tasks;
using Limbo.Umbraco.Fetch.Models.Settings;
using Limbo.Umbraco.Fetch.Services;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Umbraco.Cms.Infrastructure.HostedServices;

#pragma warning disable CS1591

namespace Limbo.Umbraco.Fetch.Scheduling;

public class FetchTask : RecurringHostedServiceBase {

    private readonly IOptions<FetchSettings> _settings;
    private readonly FetchService _fetchService;

    public FetchTask(ILogger<FetchTask> logger, IOptions<FetchSettings> settings, FetchService fetchService, TimeProvider timeProvider) : base(logger, settings.Value.Scheduling.Interval, settings.Value.Scheduling.Delay, timeProvider) {
        _settings = settings;
        _fetchService = fetchService;
    }

    public override async Task PerformExecuteAsync(CancellationToken stoppingToken) {
        if (stoppingToken.IsCancellationRequested) return;
        if (!_settings.Value.Scheduling.IsEnabled) return;
        await _fetchService.FetchAll();
    }

}