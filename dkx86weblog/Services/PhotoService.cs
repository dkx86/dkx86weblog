using dkx86weblog.Data;
using dkx86weblog.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace dkx86weblog.Services
{
    public class PhotoService
    {
        public const string PHOTOS_DIR_NAME = "photos";
        private readonly ApplicationDbContext _context;
        private readonly ImageService _imageService;
        private readonly FileSystemService _filesystemService;

        public PhotoService(ApplicationDbContext context, ImageService imageService, FileSystemService filesystemService)
        {
            _context = context;
            _imageService = imageService;
            _filesystemService = filesystemService;
        }

        internal async Task UploadAsync(Photo photo, IFormFile photoFile)
        {
            //Upload file
            string photoDirPath = _filesystemService.CreateDirIfNotExists(PHOTOS_DIR_NAME);
            var filePath = await _filesystemService.AddFileToServer(photoFile, photoDirPath);

            //Make preview file
            string previewFilePath = Path.Combine(photoDirPath, Photo.PREVIEW_PREFIX + Path.GetFileName(photoFile.FileName));
            var resizeResult = _imageService.Resize(filePath, previewFilePath, Photo.MAX_PREVIEW_LONG_SIDE);

            //save model
            photo.ID = Guid.NewGuid();
            photo.Time = DateTime.Now;
            photo.FileName = Path.GetFileName(photoFile.FileName);
            photo.Height = resizeResult.OriginalHeight;
            photo.Width= resizeResult.OriginalWidth;

            _context.Add(photo);
            await _context.SaveChangesAsync();
        }

        internal async Task<List<Photo>> LoadPhotosAsync()
        {
            return await _context.Photo.OrderByDescending(p => p.Time).ToListAsync();
        }

        internal async Task<Photo> FindPhotoAsync(Guid? id)
        {
            if (id == null)
                return null;
            return await _context.Photo.FirstOrDefaultAsync(p => p.ID == id);
        }

        internal async Task RemovePhotoAsync(Guid? id)
        {
            var photo = await FindPhotoAsync(id);
            if (photo == null)
                return;

            _context.Photo.Remove(photo);
            await _context.SaveChangesAsync();
            _filesystemService.RemoveFileFromServer(photo.GetPreviewFileName(), PHOTOS_DIR_NAME);
            _filesystemService.RemoveFileFromServer(photo.FileName, PHOTOS_DIR_NAME);
        }

        internal async Task<Photo> EditPhotoAsync(Guid id, Photo updatedPhoto)
        {
            var photo = await FindPhotoAsync(id);
            try
            {
                photo.Title = updatedPhoto.Title;
                _context.Update(photo);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                return null;
            }
            return photo;
        }

        internal bool PhotoExists(Guid id)
        {
            return _context.Photo.Any(p => p.ID == id);
        }
    }
}
