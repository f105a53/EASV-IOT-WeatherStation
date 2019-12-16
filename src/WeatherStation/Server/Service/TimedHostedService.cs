using Microsoft.Extensions.Hosting;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace WeatherStation.Server.Service
{
    public abstract class TimedHostedService : IHostedService, IDisposable
    {
        private Timer _timer;
        protected readonly ILogger _logger = Log.ForContext<TimedHostedService>();
        private readonly TimeSpan interval;

        public TimedHostedService(TimeSpan interval)
        {
            this.interval = interval;
        }

        public Task StartAsync(CancellationToken stoppingToken)
        {
            _logger.Information("Timed Hosted Service running.");

            _timer = new Timer(DoWork, null, TimeSpan.Zero, interval);

            return Task.CompletedTask;
        }

        protected abstract void DoWork(object state);

        public Task StopAsync(CancellationToken stoppingToken)
        {
            _logger.Information("Timed Hosted Service is stopping.");

            _timer?.Change(Timeout.Infinite, 0);

            return Task.CompletedTask;
        }

        public void Dispose()
        {
            _timer?.Dispose();
        }
    }
}
