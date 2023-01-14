using AppSettings.API.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace AppSettings.API.Plugins
{
    public interface IPlugin
    {
        String Key { get; }
        Dictionary<string, string> Parameters { get; set; }

        PluginResult Execute(AppSettingDatabaseResponse appSettingResponse, Dictionary<string, string> parameters);
    }
}
