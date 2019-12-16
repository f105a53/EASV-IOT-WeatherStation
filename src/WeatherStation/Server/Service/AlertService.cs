using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WeatherStation.Server.Service
{
    public class AlertService : TimedHostedService
    {
        private readonly QueryService queryService;

        public AlertService(QueryService queryService) : base(TimeSpan.FromMinutes(1))
        {
            this.queryService = queryService;
        }

        protected override void DoWork(object state)
        {
            const int warn = 60;
            var data = queryService.GetAvgHumidities().ConfigureAwait(false).GetAwaiter().GetResult();
            var warnings = data.Where(d => d.Value.Value > warn).ToList();
            if (warnings.Any())
            {
                var subj = $"Alert, high humidity on {string.Join(',', warnings.Select(d => d.Key)) }";
                var msg = $"We have detected high humidity on your device(s).\n{JsonConvert.SerializeObject(warnings.Select(d => d.Value).ToList(), Formatting.Indented)}";
            }
        }
    }
}
