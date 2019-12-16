using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WeatherStation.Server.Service;
using WeatherStation.Shared;

namespace WeatherStation.Server.Controllers
{
    [Route("/api/[controller]")]
    [ApiController]
    public class DevicesController : ControllerBase
    {
        private readonly QueryService queryService;
        private readonly DeviceRenamerService deviceRenamerService;

        public DevicesController(QueryService queryService, DeviceRenamerService deviceRenamerService)
        {
            this.queryService = queryService;
            this.deviceRenamerService = deviceRenamerService;
        }

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
        public void Post([FromBody] Rename nr)
        {
            deviceRenamerService.Rename(nr.Name,nr.RenameTo);
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
