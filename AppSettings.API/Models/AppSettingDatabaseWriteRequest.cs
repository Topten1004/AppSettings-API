using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace AppSettings.API.Models
{

    public class AppSettingDatabaseWriteRequest
    {
        public AppSettingAuthentication AppSettingAuthentication { get; set; }
        public string FileName { get; set; }
        public string UserObject { get; set; }
        public string Command { get; set; }
        public string Base64RawString { get; set; }
        public AppSettingFilter AppSettingsFilter { get; set; }
        public PluginContainer PluginContainer { get; set; }
    }
}
