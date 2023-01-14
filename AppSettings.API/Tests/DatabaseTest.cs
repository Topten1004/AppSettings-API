using AppSettings.API.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace AppSettings.API.Tests
{
    [TestClass]
    public class DatabaseTest
    {
        public static string MDF_Directory
        {
            get
            {
                var directoryPath = AppDomain.CurrentDomain.BaseDirectory;
                return Path.GetFullPath(Path.Combine(directoryPath, "..//..//..//TestDB"));
            }
        }

        public string AppSettingsConnectionString = "Data Source=(LocalDB)\\MSSQLLocalDB; " +
                "AttachDbFilename=" + MDF_Directory + "\\Astoot.mdf;" +
                " Integrated Security=True; Connect Timeout=30;";


        [TestMethod]
        public void TestMethod1()
        {
            var optionsBuilder = new DbContextOptionsBuilder<AppSettingsDatabaseContext>();
            //I've also tried UseSqlLite()
            optionsBuilder.UseSqlServer(this.AppSettingsConnectionString);
            using (var context = new AppSettingsDatabaseContext(optionsBuilder.Options))
            {
                var settings = context.AppSettingDataObjects.ToList();
                Assert.IsTrue(settings.Any());
            }
        }

     
    }
}