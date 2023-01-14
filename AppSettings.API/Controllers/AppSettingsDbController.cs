using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using AppSettings.API.Data;
using AppSettings.API.Models;
using AppSettings.API.Utilities;
using AppSettings.API.Plugins;

namespace AppSettings.API.Controllers
{
    [Route("api/Db/[controller]")]
    [ApiController]

    public class AppSettingsDbController : ControllerBase
    {
        private readonly AppSettingsDatabaseContext _context;

        public AppSettingsDbController(AppSettingsDatabaseContext context)
        {
            _context = context;
            _context.CheckTemplateItemZero();
        }

        //// GET: api/Photos
        //[HttpGet]
        //[Produces("application/json")]
        //[Route("/api/AppSettings")]
        //public IActionResult GetPhotos()
        //{
        //    List<AppSettingDataObjectViewModel> lstPhotosViewModel = new List<AppSettingDataObjectViewModel>();

        //    List<AppSettings> lstPhotos = _context.AppSettings.ToList();

        //    if (lstPhotos.Count > 0)
        //    {
        //        foreach (AppSettings photo in lstPhotos)
        //        {
        //            AppSettingDataObjectViewModel photosViewModel = new AppSettingDataObjectViewModel
        //            {

        //                Image = Convert.ToBase64String(System.IO.File.ReadAllBytes(photo.Path)),
        //                Path = photo.Path,
        //                CreatedDate = photo.CreatedDate
        //            };

        //            lstPhotosViewModel.Add(photosViewModel);
        //        }
        //    }

        //    JsonResult result = new JsonResult(lstPhotosViewModel, new JsonSerializerSettings
        //    {
        //        Formatting = Formatting.Indented,
        //    });

        //    return result;

        //}

        private AppSettingDatabaseResponse ReadFromDatabase(AppSettingFilter appSettingsFilter)
        {
            var appSettingDataObject = _context.AppSettingDataObjects.Where(
                    o => o.ApplicationName == appSettingsFilter.ApplicationName &&
                         o.RootKey == appSettingsFilter.RootKey &&
                         o.RegionKey == appSettingsFilter.RegionKey &&
                         o.PropertyName == appSettingsFilter.PropertyName).FirstOrDefault();
            return appSettingDataObject;
        }

        // GET: api/Read
        [HttpPost]
        [Produces("application/json")]
        [Route("/api/Db/Read")]
        public async Task<ActionResult<string>> Read(AppSettingDatabaseReadRequest appSettingReadRequest)
        {
            AppSettingDatabaseResponse appSettingResponse = null;
            try
            {
                appSettingResponse = ReadFromDatabase(appSettingReadRequest.AppSettingsFilter);

                //appSettingResponse.AppSettingDataDescriptor = _context.AppSettingDataDescriptor.Where(x => x.AppSettingDataObjectId == appSettingResponse.Id).First();
                appSettingResponse.AppSettingDataDescriptor = _context.AppSettingDataDescriptor.Where(x => x.Id == appSettingResponse.Id).First();
                appSettingResponse.AppSettingDataDescriptor.DateLastRead = DateTime.Now;
                appSettingResponse.UserObject = appSettingReadRequest.UserObject;
                appSettingResponse.AppSettingAuthentication = appSettingReadRequest.AppSettingAuthentication;
                var binaryFileExists = string.IsNullOrEmpty(appSettingResponse.AppSettingDataDescriptor.FileName) == false;
                if (binaryFileExists)
                {
                    appSettingResponse.Base64RawString = FileUtility.ReadFile(appSettingResponse);
                }

                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                if (appSettingResponse == null) appSettingResponse = new AppSettingDatabaseResponse();
                appSettingResponse.LastError = ex.ToString();
                //throw ex;
            }

            appSettingResponse.ApplicationName = appSettingReadRequest.AppSettingsFilter.ApplicationName;
            appSettingResponse.RootKey = appSettingReadRequest.AppSettingsFilter.RootKey;
            appSettingResponse.RegionKey = appSettingReadRequest.AppSettingsFilter.RegionKey;
            appSettingResponse.PropertyName = appSettingReadRequest.AppSettingsFilter.PropertyName;

            var result = JsonConvert.SerializeObject(appSettingResponse, Formatting.None,
                        new JsonSerializerSettings()
                        {
                            ReferenceLoopHandling = ReferenceLoopHandling.Ignore
                        });
            //JsonResult result = new JsonResult(appSettingDataObject, new JsonSerializerSettings
            //{
            //    Formatting = Formatting.Indented,
            //},
            //        new JsonSerializerSettings()
            //        { 
            //            ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            //        });

            return result;

        }



        //// POST: api/Photos
        //// To protect from overposting attacks, enable the specific properties you want to bind to, for
        //// more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        //[HttpPost]
        //[Route("/api/AppSettings")]
        //public async Task<ActionResult<AppSettingDataObject>> PostPhotos(AppSettingDataObject appSettingDataObject)
        //{
        //    try
        //    {
        //        appSettingsViewModel.Image = appSettingsViewModel.Image.Split(',')[1];
        //        string filePath = FileUtility.SaveFile(appSettingDataObject);


        //            DataPath = filePath,
        //            CreatedDate = DateTime.Now,
        //            Key1 = appSettingDataObject.Key1,
        //            Key2 = appSettingDataObject.Key2,
        //            Key3 = appSettingDataObject.Key3,
        //            Key4 = appSettingDataObject.Key4

        //        _context.AppSettings.Add(photosEntity);
        //        await _context.SaveChangesAsync();
        //        return CreatedAtAction("GetPhotos", new { id = photosEntity.Id }, appSettingsViewModel);

        //    }
        //    catch (Exception ex)
        //    {

        //        throw;
        //    }
        //}

        // GET: api/Write
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPost]
        [Produces("application/json")]
        [Route("/api/Db/WriteString")]
        public async Task<ActionResult<AppSettingDatabaseResponse>> WriteString(AppSettingAuthentication appSettingAuthentication, string ApplicationName, string RootKey, string RegionKey, string PropertyName, string value)
        {
            if (value == null) value = "";
            //_context.AppSettingDataObjects.Add(appSettingDataObject);
            var dataBytes = System.Text.Encoding.Unicode.GetBytes(value);//Convert.FromBase64String(value);
            var base64String = Convert.ToBase64String(dataBytes);
            //return await Write(appSettingAuthentication, appSettingDataObject);
            var appSettingDataValue = new AppSettingDatabaseWriteRequest()
            {
                AppSettingsFilter = new AppSettingFilter() { ApplicationName = ApplicationName, RootKey = RootKey, RegionKey = RegionKey, PropertyName = PropertyName },
                AppSettingAuthentication = appSettingAuthentication,
                Base64RawString = base64String
            };
            return await Write(appSettingDataValue);
        }

        private long Base64ToLength(AppSettingDatabaseWriteRequest appSettingWriteRequest)
        {
            if (appSettingWriteRequest == null) return 0;
            if (appSettingWriteRequest.Base64RawString == null) return 0;
            if (string.IsNullOrEmpty(appSettingWriteRequest.FileName)) return 0;
            var length = appSettingWriteRequest.Base64RawString.Length;
            var rawLength = ((4 * length / 3) + 3) & ~3;
            return rawLength;
        }


        // GET: api/Write
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPost]
        [Produces("application/json")]
        [Route("/api/Db/Write")]
        public async Task<ActionResult<AppSettingDatabaseResponse>> Write(AppSettingDatabaseWriteRequest appSettingWriteRequest)
        {
            var appSettingDataDescriptor = new AppSettingResponseDetails()
            {
                Length = Base64ToLength(appSettingWriteRequest),
                FileName = appSettingWriteRequest.FileName,
                DataLastWrite = DateTime.Now,
                DateCreated = DateTime.Now,
                DateLastRead = DateTime.Now,
                MediaType = AppSettings.API.Types.MediaTypeNames.ToMime(appSettingWriteRequest.FileName),
            };

            var appSettingResponse = new AppSettingDatabaseResponse()
            {
                AppSettingAuthentication = appSettingWriteRequest.AppSettingAuthentication,
                ApplicationName = appSettingWriteRequest.AppSettingsFilter.ApplicationName,
                RootKey = appSettingWriteRequest.AppSettingsFilter.RootKey,
                RegionKey = appSettingWriteRequest.AppSettingsFilter.RegionKey,
                PropertyName = appSettingWriteRequest.AppSettingsFilter.PropertyName,
                Base64RawString = appSettingWriteRequest.Base64RawString,
                UserObject = appSettingWriteRequest.UserObject,
                Command = appSettingWriteRequest.Command,
                AppSettingDataDescriptor = appSettingDataDescriptor, 
                PluginContainer = appSettingWriteRequest.PluginContainer
            };

            return await WriteExtended(appSettingResponse);
        }

        // GET: api/Write
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPost]
        [Produces("application/json")]
        [Route("/api/Db/WriteExtended")]
        public async Task<ActionResult<AppSettingDatabaseResponse>> WriteExtended(AppSettingDatabaseResponse appSettingResponse)
        {

            try
            {
                var appSettingsFilter = new AppSettingFilter() { ApplicationName = appSettingResponse.ApplicationName, RootKey = appSettingResponse.RootKey, RegionKey = appSettingResponse.RegionKey, PropertyName = appSettingResponse.PropertyName };
                var appSettingDataObjectExisting = ReadFromDatabase(appSettingsFilter);
                if (appSettingDataObjectExisting != null)
                {
                    // _context.AppSettingDataObjects.Remove(appSettingDataObjectExisting);
                    appSettingDataObjectExisting.Base64RawString = appSettingResponse.Base64RawString;
                    appSettingDataObjectExisting.AppSettingDataDescriptor = appSettingResponse.AppSettingDataDescriptor;
                }
                else
                {
                    _context.AppSettingDataObjects.Add(appSettingResponse);
                }

                await _context.SaveChangesAsync();
                var saveBinary = (appSettingResponse.AppSettingDataDescriptor.MediaType.Contains("native/") == false);
                if (saveBinary)
                {
                    FileUtility.SaveFile(appSettingResponse);
                }

                string filePath = FileUtility.SaveFile(appSettingResponse);
                PluginManager.Execute(appSettingResponse);
                appSettingResponse.AppSettingDataDescriptor.DataLastWrite = DateTime.Now;
                appSettingResponse.AppSettingDataDescriptor.DateCreated = DateTime.Now;
                appSettingResponse.AppSettingDataDescriptor.FileName = filePath;
                return CreatedAtAction("Read", new { id = appSettingResponse.Id }, appSettingResponse);
            }
            catch (Exception ex)
            {

                throw;
            }

            // return appSettingDataObject;
        }


    }
}
