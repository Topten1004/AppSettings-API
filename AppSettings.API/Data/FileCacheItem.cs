using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AppSettings.API.Data
{
    public class FileCacheItem
    {
        public String ApplicationName;
        public FileBasedItem Value;
        public bool HasChanged;
    }
}
