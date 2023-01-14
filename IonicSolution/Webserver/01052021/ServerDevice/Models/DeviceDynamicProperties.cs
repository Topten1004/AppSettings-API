using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ServerDevice.Models
{
    public class DeviceDynamicProperties
    {
        [Key]
        public int PropertyId { get; set; }
        
        public string DisplayText { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        public string Value { get; set; }
        public string PlaceHolder { get; set; }
        [Required]
        public string Type { get; set; }
        public bool IsReadOnly { get; set; }
    }
}

