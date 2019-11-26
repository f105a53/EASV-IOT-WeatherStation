using System;
using System.ComponentModel.Design.Serialization;
using System.Threading;
using System.Threading.Tasks;
using InfluxDB.Client.Api.Domain;
using InfluxDB.Client.Core;
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
                var influxDbClient = InfluxDB.Client.InfluxDBClientFactory.Create("192.168.2.108","root","root".ToCharArray());


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
                        .WithClientId("Client1")
                        .WithTcpServer("192.168.2.108").Build())
                    .Build();

                var mqttClient = new MqttFactory().CreateManagedMqttClient();
                mqttClient.UseApplicationMessageReceivedHandler(msg =>
                {
                    _logger.Information("Received: {topic} {msg}", msg.ApplicationMessage.Topic,
                        msg.ApplicationMessage.ConvertPayloadToString());
                    influxDbClient.GetWriteApi().WriteMeasurement(WritePrecision.S,new Temperature(){Device = msg.ApplicationMessage.Topic,Value = Convert.ToDouble(msg.ApplicationMessage.ConvertPayloadToString()), Time = DateTime.Now});
                    
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
    [Measurement("temperature")]
    public class Temperature
    {
        [Column("device", IsTag = true)] public string Device { get; set; }

        [Column("value")] public double Value { get; set; }

        [Column(IsTimestamp = true)] public DateTime Time;
    }
}