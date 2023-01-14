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
    [Route("api/[controller]")]
    [ApiController]
    public class DevicesController : ControllerBase
    {
        private readonly ServerDeviceContext _context;

        public DevicesController(ServerDeviceContext context)
        {
            _context = context;
        }

        // GET: api/Devices
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Device>>> GetDevice()
        {
            return await _context.Device.ToListAsync();
        }

        // GET: api/Devices/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Device>> GetDevice(int id)
        {
            var device = await _context.Device.FindAsync(id);

            if (device == null)
            {
                return NotFound();
            }

            return device;
        }

        // PUT: api/Devices/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutDevice(int id, Device device)
        {
            if (id != device.DeviceId)
            {
                return BadRequest();
            }

            _context.Entry(device).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!DeviceExists(id))
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

        // POST: api/Devices
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Device>> PostDevice(Device device)
        {
            _context.Device.Add(device);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetDevice", new { id = device.DeviceId }, device);
        }

        // DELETE: api/Devices/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteDevice(int id)
        {
            var device = await _context.Device.FindAsync(id);
            if (device == null)
            {
                return NotFound();
            }

            _context.Device.Remove(device);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool DeviceExists(int id)
        {
            return _context.Device.Any(e => e.DeviceId == id);
        }
    }
}
