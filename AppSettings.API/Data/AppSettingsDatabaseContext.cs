using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using AppSettings.API.Models;
using System.IO;
// https://docs.microsoft.com/en-us/ef/core/

namespace AppSettings.API.Data
{
    public class AppSettingsDatabaseContext : DbContext
    {
        public AppSettingsDatabaseContext(DbContextOptions<AppSettingsDatabaseContext> options)
            : base(options)
        {
            //CheckTemplateItemZero();
        }

        public void CheckTemplateItemZero()
        {
            try
            {
                var itemCount = AppSettingDataObjects.Count();
                if (itemCount > 0) return;
            }
            catch (Exception)
            {
            }
         
            var appSettingDataObjectZero = new AppSettingDatabaseResponse()
            {
                ApplicationName = "",
                RootKey = "",
                RegionKey = "",
                PropertyName = "",
                Base64RawString = "",
                LastError = "EMPTY",
                AppSettingDataDescriptor = new AppSettingResponseDetails()
            };
            AppSettingDataObjects.Add(appSettingDataObjectZero);
            SaveChanges();
        }

        //public DbSet<AppSettingsFilter> AppSettingsFilters { get; set; }
        public DbSet<AppSettingDatabaseResponse> AppSettingDataObjects { get; set; }
        public DbSet<AppSettingResponseDetails> AppSettingDataDescriptor { get; set; }




        public static string MDF_Directory
        {
            get
            {
                var directoryPath = AppDomain.CurrentDomain.BaseDirectory;
                var projectPath = Path.GetFullPath(Path.Combine(directoryPath, "..//..//..//..//Database"));
                Directory.CreateDirectory(projectPath);
                return projectPath;
            }
        }

        public string astootConnectionString = "Data Source=(LocalDB)\\AppSettingsDb2022; " +
                "AttachDbFilename=" + MDF_Directory + "\\AppSettingsDb2022.mdf;" +
                " Integrated Security=True; Connect Timeout=30;";
    }
}
