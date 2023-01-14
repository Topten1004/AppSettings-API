using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace AppSettings.API.Models
{

    public class AppSettingDatabaseReadRequest
    {
        [Key]
        public int Id { get; set; }
        public AppSettingAuthentication AppSettingAuthentication { get; set; }
        public string FileName { get; set; }
        public string UserObject { get; set; }
        public string DefaultValue { get; set; }
        public AppSettingFilter AppSettingsFilter { get; set; }
    }
}
