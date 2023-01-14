using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using AppSettings.API.Models;
using System.IO;
using AppSettings.API.Controllers;
// https://docs.microsoft.com/en-us/ef/core/

namespace AppSettings.API.Data
{
    public class AppSettingsFileContext
    {
        private DirectoryInfo _root;
        private FileCacheController _fileCacheController;

        public AppSettingsFileContext(DirectoryInfo root = null)
        {
            _fileCacheController = new FileCacheController();
            _root = root;
        }

        public void Initialize()
        {
            SetupRootDir();
        }

        private void SetupRootDir()
        {
            if (_root is null) _root = new DirectoryInfo(Environment.CurrentDirectory);

            var iniFile = new FileInfo(Path.Combine(Environment.CurrentDirectory, "database.ini"));
            if (iniFile.Exists)
            {
                var iniDir = File.ReadAllText(iniFile.FullName, System.Text.Encoding.UTF8);
                if (iniDir.Length > 2)
                {
                    if (iniDir.Contains(":"))
                    {
                        _root = new DirectoryInfo(iniDir);
                    }
                    else
                    {
                        _root = new DirectoryInfo(Path.Combine(Environment.CurrentDirectory, iniDir));
                    }
                }
            }
            if (!_root.Exists)
            {
                Directory.CreateDirectory(_root.FullName);
            }
        }

        public void Save()
        {

        }

        internal FileBasedItem Read(AppSettingFilter appSettingsFilter)
        {
            var fileBasedItem = _fileCacheController.Read(appSettingsFilter, true);
            return fileBasedItem;
        }

    }
}
