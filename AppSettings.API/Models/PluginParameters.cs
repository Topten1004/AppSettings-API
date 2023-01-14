using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace AppSettings.API.Models
{
    public class PluginParameters
    {
        [Key]
        public int Id { get; set; }
        public string PluginKey { get; set; }
        public List<PluginParameter> Parameters { get; set; }

    }
}
