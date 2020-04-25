using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.RegularExpressions;

namespace dkx86weblog.Models
{
    public class Post
    {
        public Guid ID { get; set; }

        [DataType(DataType.DateTime)]
        public DateTime CreateTime { get; set; }

        [DataType(DataType.DateTime)]
        public DateTime UpdateTime { get; set; }

        [StringLength(64)]
        public string Title { get; set; }

        [Column(TypeName = "text")]
        public string Body { get; set; }

        public bool Published { get; set; }

        public string GetPreview()
        {
            if (Body == null)
                return string.Empty;
            string noHTML = Regex.Replace(Body, @"<[^>]+>|&nbsp;|&.*?;", string.Empty).Trim();
            int end = Math.Min(256, noHTML.Length);
            return noHTML.Substring(0, end).TrimEnd() + "...";
        }
    }
}
