using System;
using System.Threading.Tasks;
using Limbo.Umbraco.Fetch.Services;
using Umbraco.Cms.Infrastructure.HostedServices;

#pragma warning disable CS1591

namespace Limbo.Umbraco.Fetch.Scheduling {

    public class FetchTask : RecurringHostedServiceBase {

        private readonly FetchService _fetchService;

        private static TimeSpan Period => TimeSpan.FromMinutes(1);

        private static TimeSpan Delay => TimeSpan.FromMinutes(1);

        public FetchTask(FetchService fetchService) : base(Period, Delay) {
            _fetchService = fetchService;
        }

        public override Task PerformExecuteAsync(object state) {
            _fetchService.FetchAll();
            return Task.CompletedTask;
        }

    }

}