using System;
using System.ComponentModel.DataAnnotations;

namespace dkx86weblog.Models
{
    public class Photo
    {
        public static readonly string PREVIEW_PREFIX = "thumb_";
        public static readonly int MAX_PREVIEW_LONG_SIDE = 1280; //px

        public Guid ID { get; set; }

        [StringLength(128)]
        [Display(Name = "File name")]
        public string FileName { get; set; }

        [DataType(DataType.DateTime)]
        public DateTime Time { get; set; }

        [StringLength(512)]
        public string Title { get; set; }


        // metadata 

        public int Height { get; set; }
        public int Width { get; set; }

        [StringLength(128)]
        public string CameraName { get; set; }

        [StringLength(16)]
        public string ExposureTime { get; set; }
        public int Aperture { get; set; }
        public int ISO { get; set; }

        public string GetPreviewFileName()
        {
            return PREVIEW_PREFIX + FileName;
        }

       

    }
}
