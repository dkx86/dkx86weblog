using System;
using System.ComponentModel.DataAnnotations;

namespace dkx86weblog.Models
{
    public class DigitalPackage
    {
        public Guid ID { get; set; }

        [StringLength(64)]
        [Display(Name = "Package file name")]
        public string PackageFileName { get; set; }

        [StringLength(72)]
        [Display(Name = "Preview file name")]
        public string PreviewFileName { get; set; }

        [StringLength(64)]
        [Display(Name = "Title")]
        public string Title { get; set; }

        [StringLength(256)]
        [Display(Name = "Description")]
        public string Description { get; set; }

        [Display(Name = "Upload date")]
        public DateTime UploadDate { get; set; }

        [Display(Name = "File type")]
        public FileType FileType { get; set; }
                
        [Display(Name = "File size (bytes)")]
        public long FileSize { get; set; } //bytes

        public string FileSizePretty()
        {
            double size = FileSize;
            string unitName = "B";

            if (FileSize / 1000 > 0)
            {
                size /= 1024;
                unitName = "kB";
            }

            if (FileSize / 1000000 > 0)
            {

                size /= 1024;
                unitName = "MB";
            }

            return Math.Round(size) + unitName;
        }
    }
}
