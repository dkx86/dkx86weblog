using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace dkx86weblog.Models
{
    public enum FileType
    {
        [Display(Name = "Zip")]
        ZIP = 1,
        [Display(Name = "PDF")]
        PDF = 2,
        [Display(Name = "PNG")]
        PNG = 3,
        [Display(Name = "JPEG")]
        JPEG = 4
    }
}
