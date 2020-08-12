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
            var meta = _imageService.GetImageMetadata(filePath);
            

            //save model
            photo.ID = Guid.NewGuid();
            photo.Time = DateTime.Now;
            photo.FileName = Path.GetFileName(photoFile.FileName);

            if (meta != null)
            {
                photo.Height = resizeResult.OriginalHeight;
                photo.Width = resizeResult.OriginalWidth;
                photo.CameraName = meta.Camera;
                photo.ExposureTime = meta.ExposureTime;
                photo.Aperture = meta.ExposureFNumber;
                photo.ISO = meta.ISO;
                photo.FocalLength = meta.FocalLength;
            }

            _context.Add(photo);
            await _context.SaveChangesAsync();
        }

        internal async Task ReadMetadataForAllPhotos()
        {
            var photos = await _context.Photo.ToListAsync();
            foreach(var photo in photos)
            {
                var filePath = _filesystemService.GetFilePath(PHOTOS_DIR_NAME, photo.FileName);
                var meta = _imageService.GetImageMetadata(filePath);

                if (meta == null)
                    continue;

                photo.Height = meta.Height;
                photo.Width = meta.Width;
                photo.CameraName = meta.Camera;
                photo.ExposureTime = meta.ExposureTime;
                photo.Aperture = meta.ExposureFNumber;
                photo.ISO = meta.ISO;
                photo.FocalLength = meta.FocalLength;

                _context.Update(photo);
            }
            await _context.SaveChangesAsync();
        }

        internal async Task<List<Photo>> ListPhotosForRssAsync(int itemsCount)
        {
            return await _context.Photo.OrderByDescending(p => p.Time).Take(itemsCount).ToListAsync();
        }

        internal async Task<PhotoViewModel> LoadPhotosAsync(int page)
        {
            var itemsAll = _context.Photo.OrderByDescending(p => p.Time);
            return await MakeViewModel(itemsAll, page);
        }

        private async Task<PhotoViewModel> MakeViewModel(IQueryable<Photo> itemsAll, int page)
        {
            var itemsCount = await itemsAll.CountAsync();
            var itemsForPage = await itemsAll.Skip((page - 1) * PageViewModel.PAGE_SIZE).Take(PageViewModel.PAGE_SIZE).ToListAsync();

            PageViewModel pageModel = new PageViewModel(itemsCount, page);
            return new PhotoViewModel(itemsForPage, pageModel);
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
