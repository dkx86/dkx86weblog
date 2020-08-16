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

        [Display(Name = "Camera")]
        [DisplayFormat(ConvertEmptyStringToNull = true, NullDisplayText = "-")]
        [StringLength(128)]
        public string CameraName { get; set; }

        [Display(Name = "Shutter speed")]
        [DisplayFormat(ConvertEmptyStringToNull = true, NullDisplayText = "-", DataFormatString = "{0}s")]
        [StringLength(16)]
        public string ExposureTime { get; set; }

        [Display(Name = "Aperture")]
        [DisplayFormat(NullDisplayText = "-", DataFormatString = "f/{0}")]
        public double? Aperture { get; set; }

        [Display(Name = "Focal length")]
        [DisplayFormat(NullDisplayText = "-", DataFormatString = "{0}mm")]
        public double? FocalLength { get; set; }

        [Display(Name = "ISO")]
        [DisplayFormat(NullDisplayText = "-")]
        public int? ISO { get; set; }

        public string GetPreviewFileName()
        {
            return PREVIEW_PREFIX + FileName;
        }



    }
}
