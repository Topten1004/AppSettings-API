using System;
using System.Drawing;
using System.IO;

namespace AppSettings.API.Extensions
{
    public static class FileInfoExtension
    {
        public static FileInfo NewExtension(this FileInfo value, string newFileExtension)
        {
            var path = value.Directory.FullName;
            var file = Path.GetFileNameWithoutExtension(value.Name);
            var ret = $@"{path}\{file}{newFileExtension}";
            return new FileInfo(ret);
        }

        public static Bitmap GetImage(this FileInfo value)
        {
            using (Stream BitmapStream = System.IO.File.Open(value.FullName, System.IO.FileMode.Open, FileAccess.Read))
            {
                var img = Image.FromStream(BitmapStream);
                var mBitmap = new Bitmap(img);
                return mBitmap;
            }
        }
    }
}
