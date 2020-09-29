using dkx86weblog.Models;
using dkx86weblog.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.ServiceModel.Syndication;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;

namespace dkx86weblog.Controllers
{
    public class HomeController : Controller
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly BlogService _blogService;
        private readonly PhotoService _photoService;
        private readonly DigitalPackagesService _digitalPackagesService;
        
        private readonly static int RSS_BLOG_FEED_SIZE = 12;
        private readonly static int RSS_PHOTO_FEED_SIZE = 12;


        public HomeController(BlogService blogService, PhotoService photoService, IHttpContextAccessor httpContextAccessor, 
            DigitalPackagesService digitalPackagesService)
        {
            _blogService = blogService;
            _photoService = photoService;
            _digitalPackagesService = digitalPackagesService;
            _httpContextAccessor = httpContextAccessor;
        }

        public IActionResult Index()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        [ResponseCache(Duration = 1200)]
        [HttpGet]
        public async Task<IActionResult> Rss()
        {
            var request = _httpContextAccessor.HttpContext.Request;
            var hostname = string.Concat(request.Scheme,"://",request.Host.ToUriComponent());
            
            var feed = new SyndicationFeed("「 dkx86・weblog 」", "IT・Photography・Otaku Culture", new Uri(hostname), hostname + "/Home/Rss", DateTime.Now);
            feed.Copyright = new TextSyndicationContent($"{DateTime.Now.Year} Dmitry Kuznetsov aka 「dkx86」");

            var items = new List<SyndicationItem>();
            items.AddRange(await GetBlogPosts());
            items.AddRange(await GetPhotos());
            items.AddRange(await GetDigitalPackages());
            feed.Items = items.OrderByDescending(i => i.PublishDate);

            return WriteRssToFile(feed);
        }

        [ResponseCache(Duration = 1200)]
        [HttpGet]
        public async Task<IActionResult> RssPhotoFeed()
        {
            var request = _httpContextAccessor.HttpContext.Request;
            var hostname = string.Concat(request.Scheme, "://", request.Host.ToUriComponent());

            var feed = new SyndicationFeed("「 dkx86・weblog・photo 」", "IT・Photography・Otaku Culture", new Uri(hostname), hostname + "/Home/RssPhotoFeed", DateTime.Now);
            feed.Copyright = new TextSyndicationContent($"{DateTime.Now.Year} Dmitry Kuznetsov aka 「dkx86」");

            var items = new List<SyndicationItem>();
            items.AddRange(await GetPhotosForFeed());
            feed.Items = items.OrderByDescending(i => i.PublishDate);

            return WriteRssToFile(feed);
        }

        private FileContentResult WriteRssToFile(SyndicationFeed feed)
        {
            var settings = new XmlWriterSettings
            {
                Encoding = Encoding.UTF8,
                NewLineHandling = NewLineHandling.Entitize,
                NewLineOnAttributes = false,
                Indent = true
            };

            using (var stream = new MemoryStream())
            {
                using (var xmlWriter = XmlWriter.Create(stream, settings))
                {
                    var rssFormatter = new Rss20FeedFormatter(feed, false);
                    rssFormatter.WriteTo(xmlWriter);
                    xmlWriter.Flush();
                }
                return File(stream.ToArray(), "application/rss+xml; charset=utf-8");
            }
        }

        private async Task<List<SyndicationItem>> GetPhotos()
        {
            List<SyndicationItem> items = new List<SyndicationItem>();
            var photos = await _photoService.ListPhotosForRssAsync(RSS_PHOTO_FEED_SIZE);
            foreach (var photo in photos)
            {
                var photoUrl = Url.Action("Details", "Photo", new { id = photo.ID }, HttpContext.Request.Scheme);
                var title = "Photo: ";
                var description = string.Empty;
                if(photo.Title != null)
                {
                    title += photo.Title;
                    description = photo.Title;
                }
                else
                {
                    title += photo.Time;
                }
                var syndicationItem = new SyndicationItem(title, description, new Uri(photoUrl), photo.ID.ToString(), photo.Time);
                syndicationItem.PublishDate = photo.Time;
                syndicationItem.ElementExtensions.Add(new XElement("enclosure", new XAttribute("type", "image/jpeg"), new XAttribute("url", "/photos/" + photo.GetPreviewFileName())).CreateReader());

                items.Add(syndicationItem);
            }
            return items;
        }

        private async Task<List<SyndicationItem>> GetPhotosForFeed()
        {
            List<SyndicationItem> items = new List<SyndicationItem>();
            var photos = await _photoService.ListPhotosForRssAsync(RSS_PHOTO_FEED_SIZE);
            foreach (var photo in photos)
            {
                var photoUrl = Url.Action(photo.FileName, "photos", null, HttpContext.Request.Scheme);
                var title = string.Empty;
                var description = string.Empty;
                if (photo.Title != null)
                {
                    title = photo.Title;
                    description = photo.Title;
                }
                
                var syndicationItem = new SyndicationItem(title, description, new Uri(photoUrl), photo.ID.ToString(), photo.Time);
                syndicationItem.PublishDate = photo.Time;
                syndicationItem.ElementExtensions.Add(new XElement("enclosure", new XAttribute("type", "image/jpeg"), new XAttribute("url", "/photos/" + photo.GetPreviewFileName())).CreateReader());

                items.Add(syndicationItem);
            }
            return items;
        }

        private async Task<List<SyndicationItem>> GetBlogPosts()
        {
            List<SyndicationItem> items = new List<SyndicationItem>();
            var postings = await _blogService.ListBlogForRssAsync(RSS_BLOG_FEED_SIZE);
            foreach (var post in postings)
            {
                var postUrl = Url.Action("Post", "Blog", new { id = post.ID }, HttpContext.Request.Scheme);
                var title = post.Title;
                var description = post.GetPreview();
                var syndicationItem = new SyndicationItem(title, description, new Uri(postUrl), post.ID.ToString(), post.CreateTime);
                syndicationItem.PublishDate = post.CreateTime;
                items.Add(syndicationItem);
            }
            return items;
        }

        private async Task<List<SyndicationItem>> GetDigitalPackages()
        {
            List<SyndicationItem> items = new List<SyndicationItem>();
            var packages = await _digitalPackagesService.ListPackagesForRssAsync(RSS_BLOG_FEED_SIZE);
            foreach (var pkg in packages)
            {
                var postUrl = Url.Action("Details", "Downloads", new { id = pkg.ID }, HttpContext.Request.Scheme);
                var title = pkg.Title;
                var description = pkg.Description;
                var syndicationItem = new SyndicationItem(title, description, new Uri(postUrl), pkg.ID.ToString(), pkg.UploadDate);
                syndicationItem.PublishDate = pkg.UploadDate;
                syndicationItem.ElementExtensions.Add(new XElement("enclosure", new XAttribute("type", "image/jpeg"), new XAttribute("url", "/downloads//" + pkg.PreviewFileName)).CreateReader());
                items.Add(syndicationItem);
            }
            return items;
        }
    }
}
