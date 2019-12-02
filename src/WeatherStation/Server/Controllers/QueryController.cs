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
    public class QueryController : ControllerBase
    {

        private readonly QueryService _query;

        public QueryController( QueryService query)
        {
            _query = query;
        }

        [HttpGet]
        public async Task<List<IGrouping<string, Temperature>>> GetTemperatures()
        {
            return await _query.GetTemperatures();
        }
    }
}
