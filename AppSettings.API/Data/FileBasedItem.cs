using AppSettings.API.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace AppSettings.API.Data
{
    public class FileBasedItem
    {
        [Key]
        public int Id { get; set; }
        public AppSettingAuthentication AppSettingAuthentication { get; set; }
        public AppSettingFilter AppSettingFilter { get; set; }
        //public string ApplicationName { get; set; }
        //public string BaseKey { get; set; }
        //public string SubKey { get; set; }
        //public string RegionKey { get; set; }
        //public string PropertyName { get; set; }
        //public string Path { get; set; }
        //public string Image { get; set; }
        public string Base64RawString { get; set; }
        public AppSettingResponseDetails AppSettingDataDescriptor { get; set; }
        public string LastError { get; set; }
        public DateTime LastReadTime { get; set; }
        public DateTime LastWriteTime { get; set; }

    }
}
