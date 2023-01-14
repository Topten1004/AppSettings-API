using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace AppSettings.API.Plugins
{
    public interface IConverterPlugin : IPlugin
    {
        FileInfo Input { get; set; }
        FileInfo Output { get; set; }
        PluginResult Convert();

    }
}
