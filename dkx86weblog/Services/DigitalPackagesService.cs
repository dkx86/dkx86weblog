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
    public class DigitalPackagesService
    {
        public const string PACKAGES_DIR_NAME = "downloads";
        public const string PREVIEW_PREFIX = "p_";
        public const int MAX_PREVIEW_LONG_SIDE = 512;
        private readonly ApplicationDbContext _context;
        private readonly ImageService _imageService;
        private readonly FileSystemService _filesystemService;

        public DigitalPackagesService(ApplicationDbContext context, ImageService imageService, FileSystemService filesystemService)
        {
            _context = context;
            _imageService = imageService;
            _filesystemService = filesystemService;
        }

        internal async Task CreateAsync(DigitalPackage package, IFormFile packageFile, IFormFile previewFile)
        {
            //Upload package file
            string packagesDirPath = _filesystemService.CreateDirIfNotExists(PACKAGES_DIR_NAME);
            await _filesystemService.AddFileToServer(packageFile, packagesDirPath);
            var packageFileSize = packageFile.Length;

            //Upload preview file            
            var previewFileName = PREVIEW_PREFIX + Path.GetFileNameWithoutExtension(packageFile.FileName) + Path.GetExtension(previewFile.FileName);
            var previewFilePath = await _filesystemService.AddFileToServer(previewFile, packagesDirPath, previewFileName);
            _imageService.Resize(previewFilePath, previewFilePath, MAX_PREVIEW_LONG_SIDE);

            //save model
            package.ID = Guid.NewGuid();
            package.UploadDate = DateTime.Now;
            package.PackageFileName = Path.GetFileName(packageFile.FileName);
            package.PreviewFileName = previewFileName;
            package.FileSize = packageFileSize;
            
            _context.Add(package);
            await _context.SaveChangesAsync();
        }

        internal async Task<List<DigitalPackage>> ListPackagesForRssAsync(int itemsCount)
        {
            return await _context.DigitalPackage.OrderByDescending(p => p.UploadDate).Take(itemsCount).ToListAsync();
        }

        internal async Task<DigitalPackageViewModel> LoadPackagesAsync(int page)
        {
            var itemsAll = _context.DigitalPackage.OrderByDescending(p => p.UploadDate);
            return await MakeViewModel(itemsAll, page);
        }

        private async Task<DigitalPackageViewModel> MakeViewModel(IQueryable<DigitalPackage> itemsAll, int page)
        {
            var itemsCount = await itemsAll.CountAsync();
            var itemsForPage = await itemsAll.Skip((page - 1) * PageViewModel.PAGE_SIZE).Take(PageViewModel.PAGE_SIZE).ToListAsync();

            PageViewModel pageModel = new PageViewModel(itemsCount, page);
            return new DigitalPackageViewModel(itemsForPage, pageModel);
        }


        internal async Task<DigitalPackage> EditPackageAsync(Guid id, DigitalPackage updatedPackage)
        {
            var package = await FindPackageAsync(id);
            try
            {
                package.Title = updatedPackage.Title;
                package.Description = updatedPackage.Description;
                package.FileType = updatedPackage.FileType;
                _context.Update(package);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                return null;
            }
            return package;
        }

        internal async Task RemovePackageAsync(Guid? id)
        {
            var package = await FindPackageAsync(id);
            if (package == null)
                return;

            _context.DigitalPackage.Remove(package);
            await _context.SaveChangesAsync();
            _filesystemService.RemoveFileFromServer(package.PackageFileName, PACKAGES_DIR_NAME);
            _filesystemService.RemoveFileFromServer(package.PreviewFileName, PACKAGES_DIR_NAME);
        }

        internal async Task<DigitalPackage> FindPackageAsync(Guid? id)
        {
            if (id == null)
                return null;
            return await _context.DigitalPackage.FirstOrDefaultAsync(p => p.ID == id);
        }

        private bool DigitalPackageExists(Guid id)
        {
            return _context.DigitalPackage.Any(e => e.ID == id);
        }
    }
}
