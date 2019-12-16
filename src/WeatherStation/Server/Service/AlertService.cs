using MailKit.Net.Smtp;
using MimeKit;
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

        public AlertService(QueryService queryService) : base(TimeSpan.FromMinutes(100))
        {
            this.queryService = queryService;
        }

        protected override void DoWork(object state)
        {
            const int warn = 60;
            var data = queryService.GetAvgHumidities().ConfigureAwait(false).GetAwaiter().GetResult();
            var warnings = data.Where(d => d.Value.Value > warn).ToList();
            if (warnings.Any() && false) //DISABLE IN DEV
            {
                var subj = $"Alert, high humidity on {string.Join(',', warnings.Select(d => d.Key)) }";
                var msg = $"We have detected high humidity on your device(s).\n{JsonConvert.SerializeObject(warnings.Select(d => d.Value).ToList(), Formatting.Indented)}";

                var message = new MimeMessage();
                message.From.Add(new MailboxAddress("Humidity Monitor", "no-reply@test.j2ghz.eu"));
                message.To.Add(new MailboxAddress("Jozef Holly", "j2.00ghz@gmail.com"));
                message.Subject = subj;

                message.Body = new TextPart("plain")
                {
                    Text = msg
                };

                using (var client = new SmtpClient())
                {
                    // For demo-purposes, accept all SSL certificates (in case the server supports STARTTLS)
                    client.ServerCertificateValidationCallback = (s, c, h, e) => true;

                    client.Connect("smtp.eu.sparkpostmail.com", 587, false);

                    // Note: only needed if the SMTP server requires authentication
                    client.Authenticate("SMTP_Injection", "202bae67bf7cf9a784bb2405b3e45c8f2371a0c7");

                    client.Send(message);
                    client.Disconnect(true);
                }
            }


        }
    }
}
