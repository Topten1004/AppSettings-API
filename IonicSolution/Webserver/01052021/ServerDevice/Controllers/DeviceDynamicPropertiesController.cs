using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ServerDevice.Data;
using ServerDevice.Models;

namespace ServerDevice.Controllers
{
    [Produces("application/json")]
    [Route("api/[controller]")]
    [ApiController]
    public class DeviceDynamicPropertiesController : ControllerBase
    {
        private readonly ServerDeviceContext _context;
        
        private readonly IDataRepository<DeviceDynamicProperties> _repo;
        public DeviceDynamicPropertiesController(ServerDeviceContext context, IDataRepository<DeviceDynamicProperties> repo)
        {
            _context = context;
            _repo = repo;
        }

        // GET: api/DeviceDynamicProperties
        [HttpGet]
        public async Task<ActionResult<IEnumerable<DeviceDynamicProperties>>> GetDeviceDynamicProperties()
        {
            return await _context.DeviceDynamicProperties.ToListAsync();
        }

        // GET: api/DeviceDynamicProperties/5
        [HttpGet("{id}")]
        public async Task<ActionResult<DeviceDynamicProperties>> GetDeviceDynamicProperties(int id)
        {
            var deviceDynamicProperties = await _context.DeviceDynamicProperties.FindAsync(id);

            if (deviceDynamicProperties == null)
            {
                return NotFound();
            }

            return deviceDynamicProperties;
        }

        // PUT: api/DeviceDynamicProperties/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPut("{id}")]
        public async Task<IActionResult> PutDeviceDynamicProperties(int id, DeviceDynamicProperties deviceDynamicProperties)
        {
            if (id != deviceDynamicProperties.PropertyId)
            {
                return BadRequest();
            }

            _context.Entry(deviceDynamicProperties).State = EntityState.Modified;

            try
            {
                _repo.Update(deviceDynamicProperties);
                var save = await _repo.SaveAsync(deviceDynamicProperties);
            
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!DeviceDynamicPropertiesExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/DeviceDynamicProperties
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPost]
        public async Task<ActionResult<DeviceDynamicProperties>> PostDeviceDynamicProperties(DeviceDynamicProperties deviceDynamicProperties)
        {
           _repo.Add(deviceDynamicProperties);
         var save=   await _repo.SaveAsync(deviceDynamicProperties);

            return CreatedAtAction("GetDeviceDynamicProperties", new { id = deviceDynamicProperties.PropertyId }, deviceDynamicProperties);
        }

        // DELETE: api/DeviceDynamicProperties/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<DeviceDynamicProperties>> DeleteDeviceDynamicProperties(int id)
        {
            var deviceDynamicProperties = await _context.DeviceDynamicProperties.FindAsync(id);
            if (deviceDynamicProperties == null)
            {
                return NotFound();
            }

            _repo.Delete(deviceDynamicProperties);
         var save=   await _repo.SaveAsync(deviceDynamicProperties);

            return deviceDynamicProperties;
        }

        private bool DeviceDynamicPropertiesExists(int id)
        {
            return _context.DeviceDynamicProperties.Any(e => e.PropertyId == id);
        }
    }
}
