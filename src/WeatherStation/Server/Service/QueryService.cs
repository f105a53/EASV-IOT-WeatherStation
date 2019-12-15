using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using InfluxDB.Client;
using InfluxDB.Client.Api.Domain;
using InfluxDB.Client.Core;
using WeatherStation.Shared;

namespace WeatherStation.Server.Service
{
    public class QueryService
    {
        private readonly InfluxDBClient _client;

        public QueryService()
        {
            _client = InfluxDBClientFactory.Create("https://eu-central-1-1.aws.cloud2.influxdata.com",
                "epLboaxSf0aYI6FBjeAoFb-RXykUUvwoeTt62Fc6rT414BD94zO6-2KPu6NbM5MuMdwpfp5zEK3dvnHjIKAynQ=="
                    .ToCharArray());
        }

        public async Task<IDictionary<string, List<Humidity>>> GetHumidities()
        {
            var api = _client.GetQueryApi();
            var data = await api.QueryAsync<Humidity>("from(bucket:\"humidity\") |> range(start:-12h) |> filter(fn: (r) => r._measurement == \"humidity\")","93a7785d1f9d8493");
            return data.GroupBy(d => d.Device).ToDictionary(x=>x.Key,x=>x.ToList());
        }

        public async Task<IDictionary<string, List<Temperature>>> GetTemperatures()
        {
            var api = _client.GetQueryApi();
            var data = await api.QueryAsync<Temperature>("from(bucket:\"humidity\") |> range(start:-12h) |> filter(fn: (r) => r._measurement == \"temperature_C\")","93a7785d1f9d8493");
            return data.GroupBy(d => d.Device).ToDictionary(x=>x.Key,x=>x.ToList());
        }

        public async Task<IList<string>> GetDevices()
        {
            var api = _client.GetQueryApi();
            var data = await api.QueryAsync<Temperature>("from(bucket:\"humidity\") |> range(start:-12h) ","93a7785d1f9d8493");
            return data.Select(d=>d.Device).Distinct().ToList();
        }
    }
}
