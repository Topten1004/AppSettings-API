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
    public class DeviceDetailsController : ControllerBase
    {
        private readonly ServerDeviceContext _context;

        public DeviceDetailsController(ServerDeviceContext context)
        {
            _context = context;
        }

        // GET: api/DeviceDetails
        [HttpGet]
        public async Task<ActionResult<IEnumerable<DeviceDetail>>> GetDeviceDetail()
        {
            return await _context.DeviceDetail.ToListAsync();
        }

        // GET: api/DeviceDetails/5
        [HttpGet("{id}/{dataUpdatedServerDate?}")]
        public async Task<ActionResult<DeviceDetail>> GetDeviceDetail(string id, string dataUpdatedServerDate)
        {
            var deviceDetail = await _context.DeviceDetail.FindAsync(id);

            if (deviceDetail == null)
            {
                return NotFound();
            }
            if (dataUpdatedServerDate == null)
            {
                return deviceDetail;
            }
            else if (deviceDetail.dateServerDateTimeTicks.Subtract(Convert.ToDateTime(dataUpdatedServerDate)).Ticks > 0)
            {
                return deviceDetail;
            }
            return NotFound();
        }

        // PUT: api/DeviceDetails/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutDeviceDetail(string id, DeviceDetail deviceDetail)
        {
            if (id != deviceDetail.strDeviceId)
            {
                return BadRequest();
            }
            deviceDetail.dateServerDateTimeTicks = DateTime.Now;
            _context.Entry(deviceDetail).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!DeviceDetailExists(id))
                {
                    PostDeviceDetail(deviceDetail);
                    //return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/DeviceDetails
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<DeviceDetail>> PostDeviceDetail(DeviceDetail deviceDetail)
        {
            deviceDetail.dateServerDateTimeTicks = DateTime.Now;
            _context.DeviceDetail.Add(deviceDetail);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (DeviceDetailExists(deviceDetail.strDeviceId))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            return CreatedAtAction("GetDeviceDetail", new { id = deviceDetail.strDeviceId }, deviceDetail);
        }

        // DELETE: api/DeviceDetails/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteDeviceDetail(string id)
        {
            var deviceDetail = await _context.DeviceDetail.FindAsync(id);
            if (deviceDetail == null)
            {
                return NotFound();
            }

            _context.DeviceDetail.Remove(deviceDetail);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool DeviceDetailExists(string id)
        {
            return _context.DeviceDetail.Any(e => e.strDeviceId == id);
        }
    }
}
