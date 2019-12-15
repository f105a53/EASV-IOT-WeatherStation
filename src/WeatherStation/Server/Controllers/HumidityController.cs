using WeatherStation.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using WeatherStation.Server.Service;

namespace WeatherStation.Server.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class HumidityController : Controller
    {
        private readonly QueryService _query;

        public HumidityController (QueryService query)
        {
            _query = query;
        }

        [HttpGet]
        public async Task<IDictionary<string, List<Humidity>>> GetHumidities()
        {
            return await _query.GetHumidities();
        }
    }
}