using dkx86weblog.Data;
using dkx86weblog.Models;
using dkx86weblog.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace dkx86weblog.Controllers
{
    public class PhotoController : Controller
    {
        private readonly PhotoService _photoService;

        public PhotoController(PhotoService photoService)
        {
            _photoService = photoService;
        }

        // GET: Photo
        public async Task<IActionResult> Index()
        {
            var photos = await _photoService.LoadPhotosAsync();
            return View(photos);
        }

        // GET: Photo/Manage
        [Authorize]
        public async Task<IActionResult> Manage()
        {
            var photos = await _photoService.LoadPhotosAsync();
            return View(photos);
        }

        // GET: Photo/Details/5
        public async Task<IActionResult> Details(Guid? id)
        {
            var photo = await _photoService.FindPhotoAsync(id);
            if (photo == null)
            {
                return NotFound();
            }

            return View(photo);
        }

        // GET: Photo/Upload
        [Authorize]
        public IActionResult Upload()
        {
            return View();
        }

        // POST: Photo/Upload
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Upload([Bind("Id,Title")] Photo photo, IFormFile photoFile)
        {
            if (!ModelState.IsValid || photoFile == null)
            {
                return View(photo);
            }

            await _photoService.UploadAsync(photo, photoFile);
            return RedirectToAction(nameof(Index));

        }

        // GET: Photo/Edit/5
        [Authorize]
        public async Task<IActionResult> Edit(Guid? id)
        {
            var photo = await _photoService.FindPhotoAsync(id);
            if (photo == null)
            {
                return NotFound();
            }
            return View(photo);
        }

        // POST: Photo/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, [Bind("Id,Title")] Photo photo)
        {
            if (id != photo.Id || !_photoService.PhotoExists(photo.Id))
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                if (await _photoService.EditPhotoAsync(id, photo) == null)
                {
                    return NotFound();
                }

                return RedirectToAction(nameof(Index));
            }

            return View(photo);
        }

        // GET: Photo/Delete/5
        [Authorize]
        public async Task<IActionResult> Delete(Guid? id)
        {
            var photo = await _photoService.FindPhotoAsync(id);
            if (photo == null)
            {
                return NotFound();
            }

            return View(photo);
        }

        // POST: Photo/Delete/5
        [HttpPost, ActionName("Delete")]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            var photo = await _photoService.FindPhotoAsync(id);
            await _photoService.RemovePhotoAsync(id);
            return RedirectToAction(nameof(Index));
        }


    }
}
