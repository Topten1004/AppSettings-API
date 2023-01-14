using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace AppSettings.API.Models
{
    public class PluginContainer
    {
        [Key]
        public int Id { get; set; }
        public List<PluginParameters> Plugins { get; set; }
    }
}
