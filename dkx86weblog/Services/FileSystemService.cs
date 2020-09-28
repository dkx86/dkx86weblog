using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;
using System.IO;
using System.Threading.Tasks;

namespace dkx86weblog.Services
{
    public class FileSystemService
    {
        private readonly IWebHostEnvironment _appEnvironment;
        private readonly ILogger<FileSystemService> _logger;

        public FileSystemService(IWebHostEnvironment appEnvironment, ILogger<FileSystemService> logger)
        {
            _logger = logger;
            _appEnvironment = appEnvironment;
        }

        public void RemoveFileFromServer(string fileName, string dirName)
        {
            string path = Path.Combine(_appEnvironment.WebRootPath, dirName, fileName);
            if (!File.Exists(path))
            {
                _logger.LogWarning("File {PATH} already removed", path);
                return;
            }

            try
            {
                File.Delete(path);
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message, e);
            }
        }

        public void RemoveFolderFromServer(string dirName, string subDirName)
        {
            string path = Path.Combine(_appEnvironment.WebRootPath, dirName, subDirName);
            if (!Directory.Exists(path))
            {
                _logger.LogWarning("Directory {PATH} already removed", path);
                return;
            }

            try
            {
                DeleteDirectory(path);
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message, e);
            }
        }

        public async Task<string> AddFileToServer(IFormFile file, string dirPath)
        {
            return await AddFileToServer(file, dirPath, Path.GetFileName(file.FileName));
        }

        public async Task<string> AddFileToServer(IFormFile file, string dirPath, string fileName)
        {
            string path = Path.Combine(_appEnvironment.WebRootPath, dirPath, fileName);
            using (var stream = new FileStream(path, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }
            return path;
        }

        private void DeleteDirectory(string target_dir)
        {
            string[] files = Directory.GetFiles(target_dir);
            string[] dirs = Directory.GetDirectories(target_dir);

            foreach (string file in files)
            {
                File.SetAttributes(file, FileAttributes.Normal);
                File.Delete(file);
            }

            foreach (string dir in dirs)
            {
                DeleteDirectory(dir);
            }

            Directory.Delete(target_dir, false);
        }

        internal string GetFilePath(string dirPath, string fileName)
        {
            return Path.Combine(_appEnvironment.WebRootPath, dirPath, fileName);
        }

        public string CreateDirIfNotExists(string dirPath, string subDirPath)
        {
            var dirFullPath = Path.Combine(_appEnvironment.WebRootPath, dirPath, subDirPath);
            if (!Directory.Exists(dirFullPath))
                Directory.CreateDirectory(dirFullPath);
            return dirFullPath;
        }

        public string CreateDirIfNotExists(string dirPath)
        {
            var dirFullPath = Path.Combine(_appEnvironment.WebRootPath, dirPath);
            if (!Directory.Exists(dirFullPath))
                Directory.CreateDirectory(dirFullPath);
            return dirFullPath;
        }
    }
}
