using dkx86weblog.Models;
using dkx86weblog.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
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
        private readonly ILogger<HomeController> _logger;
        private readonly BlogService _blogService;
        private readonly PhotoService _photoService;

        private readonly static int RSS_BLOG_FEED_SIZE = 12;
        private readonly static int RSS_PHOTO_FEED_SIZE = 12;


        public HomeController(ILogger<HomeController> logger, BlogService blogService, PhotoService photoService)
        {
            _logger = logger;
            _blogService = blogService;
            _photoService = photoService;
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
            var feed = new SyndicationFeed("「 dkx86・weblog 」", "IT・Photography・Otaku Culture", new Uri("http://dkx86.ru"), "http://dkx86.ru/Home/Rss", DateTime.Now);

            feed.Copyright = new TextSyndicationContent($"{DateTime.Now.Year} &copy; Dmitry Kuznetsov aka 「dkx86」");
            var items = new List<SyndicationItem>();
            var postings = await _blogService.ListBlogForRssAsync(RSS_BLOG_FEED_SIZE);
            

            foreach (var item in postings)
            {
                var postUrl = Url.Action("Post", "Blog", new { id = item.ID }, HttpContext.Request.Scheme);
                var title = item.Title;
                var description = item.GetPreview();
                var syndicationItem = new SyndicationItem(title, description, new Uri(postUrl), item.ID.ToString(), item.CreateTime);
                items.Add(syndicationItem);

            }

            var photos = await _photoService.ListPhotosForRssAsync(RSS_PHOTO_FEED_SIZE);
            foreach (var item in photos)
            {
                var photoUrl = Url.Action("Index", "Photo", null, HttpContext.Request.Scheme) + "#photo_" + item.ID;
                var title = "Photo " + item.Time;
                var description = item.Title == null? String.Empty : item.Title;
                var syndicationItem = new SyndicationItem(title, description, new Uri(photoUrl), item.ID.ToString(), item.Time);
                syndicationItem.ElementExtensions.Add(new XElement("enclosure", new XAttribute("type", "image/jpeg"), new XAttribute("url", "/photos/" + item.GetPreviewFileName())).CreateReader());

                items.Add(syndicationItem);

            }

            items.OrderByDescending(i => i.LastUpdatedTime);

            feed.Items = items;
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
    }
}
