using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace AppSettings.API.Models
{
    public class AppSettingFilter
    {
        [Key]
        public int Id { get; set; }
        public string ApplicationName { get; set; }
        public string RootKey { get; set; }
        public string SubKey { get; set; }
        public string RegionKey { get; set; }
        public string PropertyName { get; set; }

        public override string ToString()
        {
            var joinedKey = $"{ApplicationName}.{RootKey}.{SubKey}.{RegionKey}.{PropertyName}";
            return joinedKey;
        }
    }
}
