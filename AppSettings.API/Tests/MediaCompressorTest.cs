using AppSettings.API.Models;
using AppSettings.API.Plugins.MediaConverter;
using AppSettings.API.Utilities;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace AppSettings.API.Tests
{
    [TestClass]
    public class MediaCompressorTest : TestBase
    {
        AppSettingDatabaseResponse _appSettingDataObject;

        [TestInitialize]
        public void Setup()
        {
            _appSettingDataObject = new AppSettingDatabaseResponse();
            CleanupOutDir();
        }

        private void CleanupOutDir()
        {
            foreach (var file in Directory.GetFiles(OutDir))
            {
                File.Delete(file);
            }
        }

        [TestMethod]
        public void ConvertVideo_Compress_Successfully()
        {
            var compressor = new VideoCompressor();
            for (int i = 1; i <= 4; i++)
            {
                compressor.Input = new System.IO.FileInfo(TestDir + $"VideoTest{i}.mp4");
                compressor.Output = new System.IO.FileInfo(OutDir + $"VideoTest{i}_converted.mp4");
                var parameters = new Dictionary<string, string>();
                var response = compressor.Execute(_appSettingDataObject, parameters);
                Assert.IsTrue(compressor.Output.Exists, $"Converted file does not exist {compressor.Output}", compressor.Input);
                Assert.IsTrue(compressor.Output.Length > 0, $"Convertion error {response.Result}, {compressor.Output}", compressor.Input);
            }
        }


        [TestMethod]
        public void ConvertImage_Compress_Successfully()
        {
            var imageList = new[] { "ImageTest1.jpg", "ImageTest2.png", "ImageTest3.jpg", "ImageTest4.tiff", "ImageTest5.jpg" };
            var compressor = new VideoCompressor();
            foreach (var imageFile in imageList)
            {
                var fileName = Path.GetFileNameWithoutExtension(imageFile);
                compressor.Input = new System.IO.FileInfo(TestDir + $"{imageFile}");
                compressor.Output = new System.IO.FileInfo(OutDir + $"{fileName}_converted.jpg");
                var parameters = new Dictionary<string, string>();
                var response = compressor.Execute(_appSettingDataObject, parameters);
                Assert.IsTrue(compressor.Output.Exists, $"Converted file does not exist {compressor.Output}", compressor.Input);
                Assert.IsTrue(compressor.Output.Length > 0, $"Convertion error {response.Result}, {compressor.Output}", compressor.Input);
            }
        }
    }
}
