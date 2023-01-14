using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ServerDevice.Models
{
    public class Device
    {
        [Key]
        public int DeviceId { get; set; }
        public int PropertyId { get; set; }
        [Required]
        public string PropertyValue { get; set; }
    }
}
