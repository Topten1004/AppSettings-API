using AppSettings.API.Types;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AppSettings.API.Models
{
    public class AppSettingResponseDetails
    {
        // AppSettingDataDescriptor: fileName, length (Bytes), createdDate, lastReadDate, lastWriteDate, MimeType
        [Key]
        public int Id { get; set; }
        public string FileName { get; set; } = "";
        public long Length { get; set; } = 0;
        public DateTime DateCreated { get; set; } = DateTime.MinValue;
        public DateTime DateLastRead { get; set; } = DateTime.MinValue;
        public DateTime DataLastWrite { get; set; } = DateTime.MinValue;
        public string MediaType { get; set; } = MediaTypeNames.Text.Plain;

        [ForeignKey("AppSettingDataObjectId")]
        public int AppSettingDataObjectId { get; set; }
        //public AppSettingDataObject AppSettingDataObject { get; set; }


    }
}
