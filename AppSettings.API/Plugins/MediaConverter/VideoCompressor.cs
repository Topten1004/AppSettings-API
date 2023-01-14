using AppSettings.API.Extensions;
using AppSettings.API.Models;
using AppSettings.API.Utilities;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace AppSettings.API.Plugins.MediaConverter
{
    /// <summary>
    /// FFmpeg converter
    /// </summary>
    // https://ffmpeg.org/ffmpeg-filters.html#select_002c-aselect
    // sample commands: https://antmedia.io/what-is-ffmpeg/
    public class VideoCompressor : ConverterBase
    {
        public VideoCompressor() : base(nameof(VideoCompressor))
        {
        }

        public string FFmpegLocation { get; private set; } = Path.Combine(FileUtility.FindRoot().FullName, @"Plugins\MediaConverter\ffmpeg\ffmpeg.exe");

        public override PluginResult Convert()
        {
            if (Input == null) return new PluginResult() { IsValid = false, Result = string.Empty };
            var mode = Parameters.GetValueOrDefault("mode", "compress");
            //var watermark = new FileInfo(FileUtility.FindRoot() + @"\Ressources\LiveTickerBand_watermark.png");
            var watermark = new FileInfo(FileUtility.FindRoot() + @"\Ressources\SportJack_Logo_212x60px.png");
            //audioAmplifyCommand = "";
            var bitrateNormal = "23";
            var bitrateBelowNormal = "28";
            var bitrateMinimal = "35";
            var bitrateGood = "16";
            var audioMaximizerNone = "31";
            var audioMaximizerLittle = "15";
            var audioMaximizerMore = "9";
            var audioMaximizerIntensive = "3";
            var audioMaximizerCommand = $@"-af ""dynaudnorm=f=10:g={audioMaximizerMore}""";
            // ToDo: Mode and bitrates as paramenter
            var result = "";
            Directory.CreateDirectory(Output.Directory.FullName);
            var arguments = "";
            if (Output.Exists)
            {
                try
                {
                    Output.Delete();
                }
                catch (Exception)
                {

                }
            }

            var qualityJpg = "6"; // Normal range for JPEG is 2-31 with 31 being the worst quality.
            var singleFrameFile = Output.NewExtension(".jpg");


            switch (Input.Extension.ToLower())
            {
                case ".mp4":
                    // -qscale:v 2
                    //arguments = $@"-i ""{Input.FullName}"" -filter:v scale=480:-1 -c:a copy ""{Output.FullName}""";
                    //var jpgArgument = $@"-i ""{Input.FullName}"" -qscale:v 2 ""{Output.FullName}%03d.jpg""";
                    var jpgOfFirstSecondArguments = $@"-i ""{Input.FullName}"" -y -vf ""select = eq(n\, 25)"" -vframes 1 -qscale:v {qualityJpg} ""{singleFrameFile.FullName}""";
                    var jpgsEvery30Seconds = $@"-i ""{Input.FullName}"" -y -vf ""select='isnan(prev_selected_t)+gte(t-prev_selected_t\,30)' ""{Output.NewExtension("").FullName}%03d.jpg""";
                    var resultJpg = RunProcess(jpgOfFirstSecondArguments);
                    var image = singleFrameFile.GetImage();

                    var compressedFile = Output.NewExtension("_compressed.mp4");
                    var maxWidth = Math.Min(image.Width, 640);
                    var compressByImageSize = $@"-i ""{Input.FullName}"" -y -filter:v scale={maxWidth}:-1 -c:v libx264 -crf {bitrateBelowNormal} -pass 1 {audioMaximizerCommand} -c:a aac -strict -2 -b:a 80k ""{compressedFile.FullName}""";
                    var compressedByFrameAnalysis = $@"-i ""{Input.FullName}"" -y -vf mpdecimate=hi=1:lo=1:frac=1:max=0 -c:a copy ""{compressedFile.FullName}""";


                    var resultCompressed = RunProcess(compressByImageSize);

                    var watermarkPositionBottomRight = "overlay=main_w-overlay_w-5:main_h-overlay_h-5";
                    var watermarkPositionTopLeft = "overlay=5:5";
                    var watermarkPositionCenter = "overlay=main_w-overlay_w-5:main_h-overlay_h-5";
                    var watermarkPositionTopRight = "overlay=main_w-overlay_w-5:5";
                    var watermarkPositionBottomLeft = "overlay=5:main_h-overlay_h";
                    var watermarkPosition = watermarkPositionBottomRight; // https://masterjosh.com/blog/how-to-positionadd-video-watermark-ffmpeg
                    var watermarkCommand = $@"-i ""{compressedFile.FullName}"" -i ""{watermark.FullName}"" -filter_complex ""{watermarkPosition}"" -c:a copy ""{Output.FullName}""";
                    arguments = watermarkCommand;
                    var resultWatermarked = RunProcess(watermarkCommand);
                    compressedFile.Delete();
                    result = resultWatermarked;
                    break;

                case ".jpg":
                case ".png":
                case ".tiff":
                    var image1 = Input.GetImage();
                    var newWidth = Math.Min(image1.Width, 640);
                    var ratio = (float)image1.Width / image1.Height;
                    var newHeight = (int)(newWidth / ratio);
                    var jpgCompressedArguments = $@"-i ""{Input.FullName}"" -y -s {newWidth}x{newHeight} -qscale:v {qualityJpg} ""{singleFrameFile.FullName}""";
                    result = RunProcess(jpgCompressedArguments);
                    
                    break;
            }
            Output.Refresh();
            return new PluginResult() { IsValid = true, Result = result };
        }

        protected string RunProcess(string arguments)
        {
            //create a process info
            var oInfo = new ProcessStartInfo(this.FFmpegLocation, arguments)
            {
                UseShellExecute = false,
                CreateNoWindow = true,
                RedirectStandardOutput = true,
                RedirectStandardError = true
            };

            var output = string.Empty;
            try
            {
                var process = System.Diagnostics.Process.Start(oInfo);
                output = process.StandardError.ReadToEnd();
                process.WaitForExit((int)TimeSpan.FromMinutes(30).TotalMilliseconds);
                process.Close();
            }
            catch (Exception)
            {
                output = string.Empty;
            }
            return output;
        }

    }
}
