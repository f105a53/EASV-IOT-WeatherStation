using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using InfluxDB.Client;
using InfluxDB.Client.Api.Domain;
using InfluxDB.Client.Core;

namespace WeatherStation.Server.Service
{
    public class QueryService
    {
        private readonly InfluxDBClient _client;

        public QueryService()
        {
            _client = InfluxDBClientFactory.Create("https://eu-central-1-1.aws.cloud2.influxdata.com",
                "HEjjtm6ewV0f8AjQ4S7ymvpPxGeqpjbFcyCCeQF-sYO7lvH60veVT3eqf_BabQEvsK_n3l5-AGsEGVcbxTGJfw=="
                    .ToCharArray());
        }

        public async Task<List<IGrouping<string, Humidity>>> GetHumidities()
        {
            var api = _client.GetQueryApi();
            var data = await api.QueryAsync<Humidity>("from(bucket:\"humidity\") |> range(start:-12h) |> filter(fn: (r) => r._measurement == \"humidity\")","f35d566fb41e6546");
            return data.GroupBy(d => d.Device).ToList();
        }

        [Measurement("humidity")]
        public class Humidity
        {
            [Column("device", IsTag = true)] public string Device { get; set; }

            [Column("value")] public double Value { get; set; }

            [Column("time", IsTimestamp = true)] public DateTime Time { get; set; }
        }
    }
}
