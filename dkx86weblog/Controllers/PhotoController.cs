using dkx86weblog.Models;
using dkx86weblog.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
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
        public async Task<IActionResult> Index(int page = 1)
        {
            var photos = await _photoService.LoadPhotosAsync(page);
            return View(photos);
        }

        // GET: Photo/Manage
        [Authorize]
        public async Task<IActionResult> Manage(int page = 1)
        {
            var photos = await _photoService.LoadPhotosAsync(page);
            return View(photos);
        }

        // GET: Photo/Details/5
        [Authorize]
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
        public async Task<IActionResult> Upload([Bind("ID,Title")] Photo photo, IFormFile photoFile)
        {
            if (!ModelState.IsValid || photoFile == null)
            {
                return View(photo);
            }

            await _photoService.UploadAsync(photo, photoFile);
            return RedirectToAction(nameof(Manage));

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
        public async Task<IActionResult> Edit(Guid id, [Bind("ID,Title")] Photo photo)
        {
            if (id != photo.ID || !_photoService.PhotoExists(photo.ID))
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                if (await _photoService.EditPhotoAsync(id, photo) == null)
                {
                    return NotFound();
                }

                return RedirectToAction(nameof(Manage));
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
            await _photoService.RemovePhotoAsync(id);
            return RedirectToAction(nameof(Manage));
        }


    }
}
