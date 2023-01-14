using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using ServerDevice.Models;

namespace ServerDevice.Data
{
    public class ServerDeviceContext : DbContext
    {
        public ServerDeviceContext (DbContextOptions<ServerDeviceContext> options)
            : base(options)
        {

        }

        public DbSet<ServerDevice.Models.DeviceDynamicProperties> DeviceDynamicProperties { get; set; }

        public DbSet<ServerDevice.Models.Device> Device { get; set; }

        public DbSet<ServerDevice.Models.DeviceDetail> DeviceDetail { get; set; }
    }
}
