using AppSettings.API.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AppSettings.API.Plugins
{
    public class PluginBase : IPlugin
    {
        private Dictionary<string, string> _parameters;
        public Dictionary<string, string> Parameters { get => _parameters; set => _parameters = value; }

        public PluginBase(string key)
        {
            _parameters = new Dictionary<string, string>();
            _key = key;//this.GetType().DeclaringType.Name;
        }

        private string _key;
        public string Key { get => _key; }


        public virtual PluginResult Execute(AppSettingDatabaseResponse appSettingResponse, Dictionary<string, string> parameters)
        {
            return new PluginResult() { IsValid = true };
        }
    }
}
