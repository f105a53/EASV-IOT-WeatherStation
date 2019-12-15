using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WeatherStation.Server.Service;

namespace WeatherStation.Server.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class DevicesController : ControllerBase
    {
        private readonly QueryService queryService;

        public DevicesController(QueryService queryService)
        {
            this.queryService = queryService;
        }

        // GET: api/Devices
        [HttpGet]
        public async Task<IEnumerable<string>> Get()
        {
            return await queryService.GetDevices();
        }

        // GET: api/Devices/5
        [HttpGet("{id}", Name = "Get")]
        public string Get(int id)
        {
            return "value";
        }

        // POST: api/Devices
        [HttpPost]
        public void Post([FromBody] string value)
        {
        }

        // PUT: api/Devices/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE: api/ApiWithActions/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
