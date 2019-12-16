using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WeatherStation.Server.Service
{
    public class DeviceRenamerService
    {
        private readonly IDictionary<string, string> renames = new Dictionary<string,string>();

        public string GetRenameOrSame(string name)
        {
            if (renames.TryGetValue(name, out var rename))
            {
                return rename;
            }
            else
            {
                return name;
            }
        }

        public void Rename(string name, string rename)
        {
            if (string.IsNullOrWhiteSpace(rename))
            {
                renames.Remove(name);
            }
            else
            {
                renames.Add(name,rename);
            }
        }
    }
}
