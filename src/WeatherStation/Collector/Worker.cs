using System;
using System.Threading;
using System.Threading.Tasks;
using InfluxDB.Client;
using InfluxDB.Client.Api.Domain;
using InfluxDB.Client.Writes;
using Microsoft.Extensions.Hosting;
using MQTTnet;
using MQTTnet.Client.Options;
using MQTTnet.Diagnostics;
using MQTTnet.Extensions.ManagedClient;
using Serilog;

namespace Collector
{
    public class Worker : BackgroundService
    {
        private readonly ILogger _logger;

        public Worker(ILogger logger)
        {
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                var influxDbClient = InfluxDBClientFactory.Create("https://eu-central-1-1.aws.cloud2.influxdata.com",
                    "epLboaxSf0aYI6FBjeAoFb-RXykUUvwoeTt62Fc6rT414BD94zO6-2KPu6NbM5MuMdwpfp5zEK3dvnHjIKAynQ=="
                        .ToCharArray());


                MqttNetGlobalLogger.LogMessagePublished += (s, e) =>
                {
                    var trace =
                        $">> [{e.TraceMessage.Timestamp:O}] [{e.TraceMessage.ThreadId}] [{e.TraceMessage.Source}] [{e.TraceMessage.Level}]: {e.TraceMessage.Message}";
                    _logger.Verbose(e.TraceMessage.Exception, "{@traceMsg}", trace);
                };

                // Setup and start a managed MQTT client.
                var options = new ManagedMqttClientOptionsBuilder()
                    .WithAutoReconnectDelay(TimeSpan.FromMilliseconds(500))
                    .WithClientOptions(new MqttClientOptionsBuilder()
                        .WithClientId("Collector")
                        .WithTcpServer("127.0.0.1")
                        .Build())
                    .Build();

                var mqttClient = new MqttFactory().CreateManagedMqttClient();
                mqttClient.UseApplicationMessageReceivedHandler(msg =>
                {
                    _logger.Information("Received: {topic} {msg}", msg.ApplicationMessage.Topic,
                        msg.ApplicationMessage.ConvertPayloadToString());
                    using var w = influxDbClient.GetWriteApi();
                    var s = msg.ApplicationMessage.Topic;
                    var i = s.LastIndexOf('/');
                    var point = PointData.Measurement(s.Substring(i + 1))
                        .Tag("device", s.Substring(0, i))
                        .Field("value", Convert.ToDouble(msg.ApplicationMessage.ConvertPayloadToString()))
                        .Timestamp(DateTime.UtcNow, WritePrecision.S);
                    w.WritePoint("humidity", "93a7785d1f9d8493", point);
                });
                await mqttClient.SubscribeAsync(
                    new[]
                    {
                        new TopicFilterBuilder().WithTopic("rtl_433/+/devices/+/+/+/humidity").Build(),
                        new TopicFilterBuilder().WithTopic("rtl_433/+/devices/+/+/+/temperature_C").Build()
                    }
                );
                await mqttClient.StartAsync(options);
                await WhenCanceled(stoppingToken);
            }
        }

        public static Task WhenCanceled(CancellationToken cancellationToken)
        {
            var tcs = new TaskCompletionSource<bool>();
            cancellationToken.Register(s => ((TaskCompletionSource<bool>) s).SetResult(true), tcs);
            return tcs.Task;
        }
    }
}