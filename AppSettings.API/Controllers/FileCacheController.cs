using AppSettings.API.Data;
using AppSettings.API.Models;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AppSettings.API.Controllers
{
    public class FileCacheController
    {
        private ConcurrentBag<FileCacheItem> _cacheItems;

        public FileCacheController()
        {
            _cacheItems = new ConcurrentBag<FileCacheItem>();
        }

        internal FileBasedItem Write(FileBasedItem fileBasedItem)
        {
            var existing = Read(fileBasedItem.AppSettingFilter, false);
            if (existing == null)
            {
                var cachedItem = new FileCacheItem()
                {
                    ApplicationName = fileBasedItem.AppSettingFilter.ApplicationName,
                    Value = fileBasedItem,
                    HasChanged = true
                };
                _cacheItems.Add(cachedItem);
                existing = cachedItem.Value;
            }
            existing.LastWriteTime = DateTime.UtcNow;
            return existing;
        }


        internal FileBasedItem Read(AppSettingFilter appSettingsFilter, bool updateReadTime)
        {
            var fileBasedItem = _cacheItems.Where(
                o => o.ApplicationName == appSettingsFilter.ApplicationName).Select(g => g.Value).Where(
                o => o.AppSettingFilter.ToString() == appSettingsFilter.ToString()).FirstOrDefault();
            if (updateReadTime)
            {
                fileBasedItem.LastReadTime = DateTime.Now;
            }
            return fileBasedItem;
        }
    }
}
