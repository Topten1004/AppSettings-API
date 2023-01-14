using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AppSettings.API.Types
{
    // Summary:
    //     Specifies the media type information for an e-mail message attachment.
    public static class MediaTypeNames //: System.Net.Mime.MediaTypeNames
    {
        public static string ToExtension(string mediaType, string originalFileName)
        {
            var ret = "dat";
            if (mediaType.Contains("native/"))
            {
                mediaType = $"/{System.IO.Path.GetExtension(originalFileName.ToLower()).TrimStart('.')}";
                //{ ret = "native"; }
            }
            if (MediaTypeNames.Application.Exe.EndsWith(mediaType)) { ret = "exe"; }
            if (MediaTypeNames.Application.Pdf.EndsWith(mediaType)) { ret = "pdf"; }
            if (MediaTypeNames.Application.Rtf.EndsWith(mediaType)) { ret = "rtf"; }
            if (MediaTypeNames.Application.Zip.EndsWith(mediaType)) { ret = "zip"; }
            if (MediaTypeNames.Image.Gif.EndsWith(mediaType)) { ret = "gif"; }
            if (MediaTypeNames.Image.Jpeg.EndsWith(mediaType)) { ret = "jpg"; }
            if ("image/jpg".EndsWith(mediaType)) { ret = "jpg"; }
            if (MediaTypeNames.Image.Tiff.EndsWith(mediaType)) { ret = "tif"; }
            if (MediaTypeNames.Image.Png.EndsWith(mediaType)) { ret = "png"; }
            if (MediaTypeNames.Video.Avi.EndsWith(mediaType)) { ret = "avi"; }
            if (MediaTypeNames.Video.Mov.EndsWith(mediaType)) { ret = "mov"; }
            if (MediaTypeNames.Video.Mp4.EndsWith(mediaType)) { ret = "mp4"; }
            if (MediaTypeNames.Video.Ogv.EndsWith(mediaType)) { ret = "ogv"; }
            if (MediaTypeNames.Text.Html.EndsWith(mediaType)) { ret = "html"; }
            if (MediaTypeNames.Text.Plain.EndsWith(mediaType)) { ret = "txt"; }
            if (MediaTypeNames.Text.RichText.EndsWith(mediaType)) { ret = "rtf"; }
            if (MediaTypeNames.Text.Xml.EndsWith(mediaType)) { ret = "xml"; }
            //if (mediaType.EndsWith(MediaTypeNames.Application.Exe)) { ret = "exe"; }
            //if (mediaType.EndsWith(MediaTypeNames.Application.Pdf)) { ret = "pdf"; }
            //if (mediaType.EndsWith(MediaTypeNames.Application.Rtf)) { ret = "rtf"; }
            //if (mediaType.EndsWith(MediaTypeNames.Application.Zip)) { ret = "zip"; }
            //if (mediaType.EndsWith(MediaTypeNames.Image.Gif)) { ret = "gif"; }
            //if (mediaType.EndsWith(MediaTypeNames.Image.Jpeg)) { ret = "jpg"; }
            //if (mediaType.EndsWith(MediaTypeNames.Image.Tiff)) { ret = "tif"; }
            //if (mediaType.EndsWith(MediaTypeNames.Image.Png)) { ret = "png"; }
            //if (mediaType.EndsWith(MediaTypeNames.Video.Avi)) { ret = "avi"; }
            //if (mediaType.EndsWith(MediaTypeNames.Video.Mov)) { ret = "mov"; }
            //if (mediaType.EndsWith(MediaTypeNames.Video.Mp4)) { ret = "mp4"; }
            //if (mediaType.EndsWith(MediaTypeNames.Video.Mpg)) { ret = "mpg"; }
            //if (mediaType.EndsWith(MediaTypeNames.Video.Ogv)) { ret = "ogv"; }
            //if (mediaType.EndsWith(MediaTypeNames.Video.Webm)) { ret = "webm"; }
            //if (mediaType.EndsWith(MediaTypeNames.Text.Html)) { ret = "html"; }
            //if (mediaType.EndsWith(MediaTypeNames.Text.Plain)) { ret = "txt"; }
            //if (mediaType.EndsWith(MediaTypeNames.Text.RichText)) { ret = "rtf"; }
            //if (mediaType.EndsWith(MediaTypeNames.Text.Xml)) { ret = "xml"; }
            return ret;
        }

        public static string ToMime(string fileName)
        {
            var ret = MediaTypeNames.Native.String;
            if (fileName == null) return ret;
            var extension = System.IO.Path.GetExtension(fileName);
            extension = extension.ToLower().Trim('.').Trim();
            if (extension == "exe") { ret = MediaTypeNames.Application.Exe; }
            if (extension == "pdf") { ret = MediaTypeNames.Application.Pdf; }
            if (extension == "zip") { ret = MediaTypeNames.Application.Zip; }
            if (extension == "gif") { ret = MediaTypeNames.Image.Gif; }
            if (extension == "jpg") { ret = MediaTypeNames.Image.Jpeg; }
            if (extension == "tif") { ret = MediaTypeNames.Image.Tiff; }
            if (extension == "png") { ret = MediaTypeNames.Image.Png; }
            if (extension == "avi") { ret = MediaTypeNames.Video.Avi; }
            if (extension == "mov") { ret = MediaTypeNames.Video.Mov; }
            if (extension == "mp4") { ret = MediaTypeNames.Video.Mp4; }
            if (extension == "mpg") { ret = MediaTypeNames.Video.Mpg; }
            if (extension == "ogv") { ret = MediaTypeNames.Video.Ogv; }
            if (extension == "webm") { ret = MediaTypeNames.Video.Webm; }
            if (extension == "html") { ret = MediaTypeNames.Text.Html; }
            if (extension == "txt") { ret = MediaTypeNames.Text.Plain; }
            if (extension == "rtf") { ret = MediaTypeNames.Text.RichText; }
            if (extension == "xml") { ret = MediaTypeNames.Text.Xml; }
            return ret;
        }

        // Summary:
        //     Specifies the kind of application data in an e-mail message attachment.
        public static class Native
        {
            // Summary:
            //     Specifies that the System.Net.Mime.MediaTypeNames.Application data is a boolean value.
            public const string Boolean = "native/boolean";
            //
            // Summary:
            //     Specifies that the System.Net.Mime.MediaTypeNames.Application data is a byte value.
            public const string Byte = "native/byte";
            //
            // Summary:
            //     Specifies that the System.Net.Mime.MediaTypeNames.Application data is a Int16 value
            public const string Int16 = "native/Int16";
            //
            // Summary:
            //     Specifies that the System.Net.Mime.MediaTypeNames.Application data is a Int32 value
            public const string Int32 = "native/Int32";
            //
            // Summary:
            //     Specifies that the System.Net.Mime.MediaTypeNames.Application data is a Int64 value
            public const string Int64 = "native/Int64";
            //
            // Summary:
            //     Specifies that the System.Net.Mime.MediaTypeNames.Application data is a string value
            public const string String = "native/String";
            //
            // Summary:
            //     Specifies that the System.Net.Mime.MediaTypeNames.Application data is a object value
            public const string Object = "native/Object";
        }


        // Summary:
        //     Specifies the kind of application data in an e-mail message attachment.
        public static class Application
        {
            // Summary:
            //     Specifies that the System.Net.Mime.MediaTypeNames.Application data is not
            //     interpreted.
            public const string Octet = "application/octet-stream";
            //
            // Summary:
            //     Specifies that the System.Net.Mime.MediaTypeNames.Application data is in
            //     Portable Document Format (PDF).
            public const string Pdf = "application/pdf";
            //
            // Summary:
            //     Specifies that the System.Net.Mime.MediaTypeNames.Application data is in
            //     Rich Text Format (RTF).
            public const string Rtf = "application/rtf";
            //
            // Summary:
            //     Specifies that the System.Net.Mime.MediaTypeNames.Application data is a SOAP
            //     document.
            public const string Soap = "application/soap+xml";
            //
            // Summary:
            //     Specifies that the System.Net.Mime.MediaTypeNames.Application data is compressed.
            public const string Zip = "application/zip";
            // 
            //     Specifies that the System.Net.Mime.MediaTypeNames.Application data is compressed.
            public const string Exe = "application/exe";
        }

        // Summary:
        //     Specifies the type of image data in an e-mail message attachment.
        public static class Image
        {
            // Summary:
            //     Specifies that the System.Net.Mime.MediaTypeNames.Image data is in Graphics
            //     Interchange Format (GIF).
            public const string Gif = "image/gif";
            //
            // Summary:
            //     Specifies that the System.Net.Mime.MediaTypeNames.Image data is in Joint
            //     Photographic Experts Group (JPEG) format.
            public const string Jpeg = "image/jpeg";
            //
            // Summary:
            //     Specifies that the System.Net.Mime.MediaTypeNames.Image data is in PNG format.
            public const string Png = "image/png";
            //
            // Summary:
            //     Specifies that the System.Net.Mime.MediaTypeNames.Image data is in Tagged
            //     Image File Format (TIFF).
            public const string Tiff = "image/tiff";
        }

        // Summary:
        //     Specifies the type of image data in an e-mail message attachment.
        public static class Video
        {
            // Summary:
            //     Specifies that the System.Net.Mime.MediaTypeNames.Video data is in Tagged
            //     Video File Format (Avi).
            public const string Avi = "video/x-msvideo";
            //
            // Summary:
            //     Specifies that the System.Net.Mime.MediaTypeNames.Video data is in Tagged
            //     Video File Format (Mov).
            public const string Mov = "video/quicktime";
            //
            // Summary:
            //     Specifies that the System.Net.Mime.MediaTypeNames.Video data is in Tagged
            //     Video File Format (Mpg).
            public const string Mpg = "video/mpeg";
            //
            // Summary:
            //     Specifies that the System.Net.Mime.MediaTypeNames.Video data is in Tagged
            //     Video File Format (Mp4).
            public const string Mp4 = "video/mp4";
            //
            // Summary:
            //     Specifies that the System.Net.Mime.MediaTypeNames.Video data is in Tagged
            //     Video File Format (Webm).
            public const string Webm = "video/webm";
            //
            // Summary:
            //     Specifies that the System.Net.Mime.MediaTypeNames.Video data is in Tagged
            //     Video File Format (Ogv).
            public const string Ogv = "video/ogv";
        }

        // Summary:
        //     Specifies the type of text data in an e-mail message attachment.
        public static class Text
        {
            // Summary:
            //     Specifies that the System.Net.Mime.MediaTypeNames.Text data is in HTML format.
            public const string Html = "text/html";
            //
            // Summary:
            //     Specifies that the System.Net.Mime.MediaTypeNames.Text data is in plain text
            //     format.
            public const string Plain = "text/plain";
            //
            // Summary:
            //     Specifies that the System.Net.Mime.MediaTypeNames.Text data is in Rich Text
            //     Format (RTF).
            public const string RichText = "text/richtext";
            //
            // Summary:
            //     Specifies that the System.Net.Mime.MediaTypeNames.Text data is in XML format.
            public const string Xml = "text/xml";
        }


    }
}
