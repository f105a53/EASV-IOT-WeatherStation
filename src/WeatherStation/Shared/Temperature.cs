using System;
using System.Collections.Generic;
using System.Text;
using InfluxDB.Client.Core;

namespace WeatherStation.Shared
{
    [Measurement("temperature_C")]
    public class Temperature
    {
        [Column("device", IsTag = true)] public string Device { get; set; }

        [Column("value")] public double Value { get; set; }

        [Column("time", IsTimestamp = true)] public DateTime Time { get; set; }
    }
}
