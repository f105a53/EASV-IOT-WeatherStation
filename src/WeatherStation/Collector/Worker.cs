using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using MQTTnet;
using MQTTnet.Client.Connecting;
using MQTTnet.Client.Options;
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

        public static Task WhenCanceled( CancellationToken cancellationToken)
        {
            var tcs = new TaskCompletionSource<bool>();
            cancellationToken.Register(s => ((TaskCompletionSource<bool>)s).SetResult(true), tcs);
            return tcs.Task;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                
                // Setup and start a managed MQTT client.
                var options = new ManagedMqttClientOptionsBuilder()
                    .WithAutoReconnectDelay(TimeSpan.FromMilliseconds(500))
                    .WithClientOptions(new MqttClientOptionsBuilder()
                        .WithClientId("Client1")
                        .WithTcpServer("192.168.2.108").Build())
                    .Build();
                
                var mqttClient = new MqttFactory().CreateManagedMqttClient();
                //mqttClient.ApplicationMessageProcessedHandler = new ApplicationMessageProcessedHandlerDelegate(args => Console.WriteLine($"msg: {args}"));
                //mqttClient.ApplicationMessageSkippedHandler = new ApplicationMessageSkippedHandlerDelegate(args => Console.WriteLine($"msg: {args}"));
                //mqttClient.SynchronizingSubscriptionsFailedHandler = new SynchronizingSubscriptionsFailedHandlerDelegate(args => Console.WriteLine($"e: {args.Exception}"));
                //mqttClient.ConnectingFailedHandler = new ConnectingFailedHandlerDelegate(args => Console.WriteLine($"e: {args.Exception}"));
                //mqttClient.ConnectedHandler = new MqttClientConnectedHandlerDelegate(args =>
                //{
                //    Console.WriteLine($"msg: {args.AuthenticateResult}");
                //});
                mqttClient.UseApplicationMessageReceivedHandler(msg => Console.WriteLine(msg.ApplicationMessage.ConvertPayloadToString()));
                await mqttClient.SubscribeAsync(new TopicFilterBuilder().WithTopic("#").Build());
                await mqttClient.StartAsync(options);
                Console.WriteLine(mqttClient.IsConnected);
                await WhenCanceled(stoppingToken);
            }
        }
    }
}
