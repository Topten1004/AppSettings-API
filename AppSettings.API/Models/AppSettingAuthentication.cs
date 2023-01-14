using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace AppSettings.API.Models
{

    public class AppSettingAuthentication
    {
        [Key]
        public int Id { get; set; }
        public string ApiKey { get; set; }
        public string Secret { get; set; }
        public string Hash { get; set; }
        public string Version { get; set; }
        public string Message { get; set; }
    }
}
