using dkx86weblog.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.IO;
using System.Threading.Tasks;

namespace dkx86weblog.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class UploadController : ControllerBase
    {

        private readonly FileSystemService _filesystemService;
        private readonly ImageService _imageService;
        private readonly static string FILES_DIR = "blog_files";
        private readonly static int MAX_BLOG_IMAGE_WIDTH = 1280;

        public UploadController(FileSystemService fileSystemService, ImageService imageService)
        {
            _filesystemService = fileSystemService;
            _imageService = imageService;
        }


        // POST: api/UploadImage/
        [Authorize]
        [HttpPost]
        public async Task<Location> UploadBlogImage(IFormFile file)
        {
            string fileName = DateTime.Now.ToFileTime() + Path.GetExtension(file.FileName);
            //Upload file
            string photoDirPath = _filesystemService.CreateDirIfNotExists(FILES_DIR);
            var filePath = await _filesystemService.AddFileToServer(file, photoDirPath, fileName);

            //Resize
            _imageService.ResizeByWidth(filePath, filePath, MAX_BLOG_IMAGE_WIDTH);

            return new Location { FileName = fileName };
        }

    }

    public class Location
    {
        public string FileName { get; set; }
    }
}
