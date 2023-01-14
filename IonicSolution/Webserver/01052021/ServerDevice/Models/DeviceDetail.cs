using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ServerDevice.Models
{
    public class DeviceDetail
    {
        [Key]
        public string strDeviceId { get; set; }
        public string strDeviceIds { get; set; }
        public string strPassword { get; set; }
        public float strVolume { get; set; }
        public bool strShowDubugPanel { get; set; }
        public bool boolUseSampling { get; set; }
        public bool boolIsOnline { get; set; }
        public byte bytFaceDetectionQuality { get; set; }
        public byte bytFaceDetectionMode { get; set; }
        public ushort ushortFaceDetectionFrameRateMaximum { get; set; }
        public bool boolQrCodeActive { get; set; }
        public byte byteDetectionCount { get; set; }
        public DateTime dateClientDateTimeTicks { get; set; }
        public DateTime dateServerDateTimeTicks { get; set; }
    }
}
