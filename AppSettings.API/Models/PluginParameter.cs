using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace AppSettings.API.Models
{
    public class PluginParameter
    {
        [Key]
        public int Id { get; set; }
        public string Key { get; set; }
        public string Value { get; set; }
    }
}
