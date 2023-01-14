using AppSettings.API.Extensions;
using AppSettings.API.Models;
using AppSettings.API.Types;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace AppSettings.API.Utilities
{
    public class FileUtility
    {
        public static string RootSystemDirectory = Directory.GetCurrentDirectory();
        public static string RootBinaryDirectory = RootSystemDirectory + @"\BinaryData\";

        public static string GetFileName(AppSettingDatabaseResponse appSettingDataObject)
        {
            var extension = MediaTypeNames.ToExtension(appSettingDataObject.AppSettingDataDescriptor.MediaType, appSettingDataObject.AppSettingDataDescriptor.FileName);
            var fileNameOnly = Path.GetFileNameWithoutExtension(appSettingDataObject.AppSettingDataDescriptor.FileName);
            //var myfilename = $"BINARY_{appSettingDataObject.Id: 00000000}"; //string.Format(@"{0}", Guid.NewGuid());
            var storageFileName = $"{appSettingDataObject.Id:00000000}_{fileNameOnly}"; //string.Format(@"{0}", Guid.NewGuid());
            var directory = RootBinaryDirectory;

            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }

            var filePath = $"{directory}{storageFileName}.{extension}";
            return filePath;
        }

        private static bool IsFileValid(AppSettingDatabaseResponse appSettingDataObject)
        {
            if (string.IsNullOrWhiteSpace(appSettingDataObject.AppSettingDataDescriptor.FileName)) return false;
            if (appSettingDataObject.AppSettingDataDescriptor.FileName == "string") return false;
            return true;
        }

        public static DirectoryInfo FindRoot()
        {
            var initialPath = Directory.GetCurrentDirectory();
            var d = new DirectoryInfo(initialPath);
            while (d.Parent.Available())
            {
                if (d.Name == "AppSettings.API")
                {
                    return d;
                }
                d = d.Parent;
            }
            return null;
        }

        public static string ReadFile(AppSettingDatabaseResponse appSettingDataObject)
        {
            if (IsFileValid(appSettingDataObject) == false) appSettingDataObject.AppSettingDataDescriptor.FileName = GetFileName(appSettingDataObject);
            var fileName = GetFileName(appSettingDataObject);
            var fileInfo = new System.IO.FileInfo(fileName);
            //if (System.IO.File.Exists(appSettingDataObject.AppSettingDataDescriptor.FileName) == false) throw new FileNotFoundException(appSettingDataObject.AppSettingDataDescriptor.FileName,; 
            var data = System.IO.File.ReadAllBytes(fileInfo.FullName);
            var base64String = Convert.ToBase64String(data, Base64FormattingOptions.None);
            return base64String;
        }


        public static string SaveFile(AppSettingDatabaseResponse appSettingDataObject)
        {
            var filePath = GetFileName(appSettingDataObject);
            var baseString = appSettingDataObject.Base64RawString;
            if (baseString.StartsWith("data:") && baseString.Contains("base64,")) baseString = baseString.Split("base64,")[1];
            byte[] dataBytes = null;
            try
            {
                dataBytes = Convert.FromBase64String(baseString);
            }
            catch (System.FormatException ex)
            {
                dataBytes = System.Text.Encoding.Unicode.GetBytes(baseString);
            }
            catch (Exception ex)
            {
                ex = ex;
            }

            using (var dataFile = new FileStream(filePath, FileMode.Create))
            {
                dataFile.Write(dataBytes, 0, dataBytes.Length);
                dataFile.Flush();
            }

            return filePath;
        }
    }
}
