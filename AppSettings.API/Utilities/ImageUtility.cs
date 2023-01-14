using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace AppSettings.API.Utilities
{
    public class ImageUtility
    {
        public static string SaveImage(string base64Image)
        {


            //this is a simple white background image
            var myfilename = string.Format(@"{0}", Guid.NewGuid());

            //Generate unique filename
            string filePath = Directory.GetCurrentDirectory() + "/UserImages/";

            if (!Directory.Exists(filePath))
            {
                Directory.CreateDirectory(filePath);
            }
            var bytess = Convert.FromBase64String(base64Image);
            filePath += myfilename + ".jpeg";
            using (var imageFile = new FileStream(filePath, FileMode.Create))
            {
                imageFile.Write(bytess, 0, bytess.Length);
                imageFile.Flush();
            }

            return filePath;
        }
    }
}
