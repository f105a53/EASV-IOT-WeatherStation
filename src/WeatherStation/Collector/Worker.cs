using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using MQTTnet;
using MQTTnet.Client.Options;
using MQTTnet.Diagnostics;
using MQTTnet.Extensions.ManagedClient;

namespace Collector
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;

        public Worker(ILogger<Worker> logger)
        {
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {

                MqttNetGlobalLogger.LogMessagePublished += (s, e) =>
                {
                    var trace = $">> [{e.TraceMessage.Timestamp:O}] [{e.TraceMessage.ThreadId}] [{e.TraceMessage.Source}] [{e.TraceMessage.Level}]: {e.TraceMessage.Message}";
                    if (e.TraceMessage.Exception != null)
                    {
                        trace += Environment.NewLine + e.TraceMessage.Exception.ToString();
                    }

                    Console.WriteLine(trace);
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
                    Console.WriteLine(msg.ApplicationMessage.ConvertPayloadToString()));
                await mqttClient.SubscribeAsync(
                    new[]
                    {
                        new TopicFilterBuilder().WithTopic("rtl_433/+/devices/+/+/+/humidity").Build()
                    }
                );
                await mqttClient.StartAsync(options);
                Console.WriteLine(mqttClient.IsConnected);
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