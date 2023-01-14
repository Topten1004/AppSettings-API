using AppSettings.API.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Linq;
using AppSettings.API.Plugins.MediaConverter;
using AppSettings.API.Extensions;

namespace AppSettings.API.Plugins
{
    public class PluginManager
    {
        private static List<IPlugin> InstalledPlugins;
        private static char pluginSplitter = '|';
        private static char valueSplitter = '_';
        private static char parameterSplitter = ',';

        internal static void Execute(AppSettingDatabaseResponse appSettingResponse)
        {

            if (appSettingResponse.PluginContainer == null) return;
            if (appSettingResponse.PluginContainer.Plugins.Count == 0) return;
            if (InstalledPlugins == null)
            {
                InstalledPlugins = new List<IPlugin>();
                InstalledPlugins.Add(new VideoCompressor());
            }
            //var pluginRequests = command.Split(pluginSplitter).ToList();
            //foreach (var p in part.Split(new[] { '|' },
            //                                 StringSplitOptions.RemoveEmptyEntries))
            var requestedPlugins = appSettingResponse.PluginContainer.Plugins;
            requestedPlugins.ForEach(o =>
            {
                var installedPlugin = InstalledPlugins.Where(o => o.Key.ToLower() == o.Key.ToLower()).FirstOrDefault();
                if (installedPlugin.Available())
                {
                    var keyValues =  o.Parameters.GroupBy(w => w.Key).ToDictionary(keySelector: g => g.Key, elementSelector: g => g.Key);
                    installedPlugin.Execute(appSettingResponse, keyValues);
                }
                //var splitterIndex = o.IndexOf(valueSplitter);
                //var pluginName = o.Split(valueSplitter).First();
                //var pluginValue = o.Substring(splitterIndex + 1);
                //var parameters = pluginValue.Split(parameterSplitter)
                //            .Select(x => x.Split(valueSplitter))
                //            .ToDictionary(x => x[0], x => x[1]);
                //if (!string.IsNullOrEmpty(pluginName))
                //{
                //    InstalledPlugins.ForEach(p =>
                //    {
                //        if (p.Key.ToLower() == pluginName.ToLower())
                //        {
                //            p.Execute(appSettingResponse, pluginValue);
                //        }
                //    });
                //}
            });
        }
    }
}
