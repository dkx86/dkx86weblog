using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.RegularExpressions;

namespace dkx86weblog.Models
{
    public class Post
    {
        private readonly int PREVIEW_MIN_LENGTH = 256;
        public Guid ID { get; set; }

        [DataType(DataType.DateTime)]
        public DateTime CreateTime { get; set; }

        [DataType(DataType.DateTime)]
        public DateTime UpdateTime { get; set; }

        [StringLength(128)]
        public string Title { get; set; }

        [Column(TypeName = "text")]
        public string Body { get; set; }

        public bool Published { get; set; }

        public string GetPreview()
        {
            if (Body == null)
                return string.Empty;
            string noHTML = Regex.Replace(Body, @"<[^>]+>|&nbsp;|&.*?;", string.Empty).Trim();
            if (noHTML.Length <= PREVIEW_MIN_LENGTH)
                return noHTML;

            int end = noHTML.IndexOf(".", PREVIEW_MIN_LENGTH) + 1;
            return noHTML.Substring(0, end).TrimEnd();
        }
    }
}
