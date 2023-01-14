using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace AppSettings.API.Models
{

    public class AppSettingDatabaseResponse
    {
        // AppSettingDataObject: rawDataString (string), AppSettingDataDescriptor
        [Key]
        public int Id { get; set; }
        public AppSettingAuthentication AppSettingAuthentication { get; set; }
        public string ApplicationName { get; set; }
        public string RootKey { get; set; }
        public string RegionKey { get; set; }
        public string UserObject { get; set; }
        public string Command { get; set; }
        public string PropertyName { get; set; }
        //public string Path { get; set; }
        //public string Image { get; set; }
        public string Base64RawString { get; set; }
        public AppSettingResponseDetails AppSettingDataDescriptor { get; set; }
        public string LastError { get; set; }
        public PluginContainer PluginContainer { get; set; }

    }
}
