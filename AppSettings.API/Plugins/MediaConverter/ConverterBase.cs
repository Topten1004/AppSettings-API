using AppSettings.API.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using AppSettings.API.Extensions;
using AppSettings.API.Utilities;

namespace AppSettings.API.Plugins.MediaConverter
{
    public abstract class ConverterBase : PluginBase, IConverterPlugin
    {
        public ConverterBase(string key) : base(key)
        {
        }

        private FileInfo _input;
        public FileInfo Input { get => _input; set => _input = value; }
        private FileInfo _output;
        public FileInfo Output { get => _output; set => _output = value; }

        public abstract PluginResult Convert();

        public override PluginResult Execute(AppSettingDatabaseResponse appSettingResponse, Dictionary<string, string> parameters)
        {
            var retExe = base.Execute(appSettingResponse, parameters);
            var relativeSource = parameters.GetValueOrDefault(nameof(Input), null);
            if (relativeSource.Available())
            {
                Input = new FileInfo(FileUtility.GetFileName(appSettingResponse));
                Output = new FileInfo(Path.Combine(Input.Directory.FullName, GetType().DeclaringType.Name, Input.Name));
                Directory.CreateDirectory(Output.Directory.FullName);
            }
            var ret = Convert();
            return ret;
        }
    }
}
