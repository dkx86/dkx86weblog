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
    public class DownloadsController : Controller
    {
        private readonly DigitalPackagesService _digitalPackageService;

        public DownloadsController(DigitalPackagesService digitalPackageService)
        {
            _digitalPackageService = digitalPackageService;
        }

        // GET: Downloads
        public async Task<IActionResult> Index(int page = 1)
        {
            return View(await _digitalPackageService.LoadPackagesAsync(page));
        }

        // GET: Downloads/Manage
        [Authorize]
        public async Task<IActionResult> Manage(int page = 1)
        {
            return View(await _digitalPackageService.LoadPackagesAsync(page));
        }

        // GET: Downloads/Details/5
        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var digitalPackage = await _digitalPackageService.FindPackageAsync(id);
            if (digitalPackage == null)
            {
                return NotFound();
            }

            return View(digitalPackage);
        }

        // GET: Downloads/Create
        [Authorize]
        public IActionResult Create()
        {
            return View();
        }

        // POST: Downloads/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ID,Title,Description,FileType")] DigitalPackage digitalPackage, IFormFile packageFile, IFormFile previewFile)
        {
            if (ModelState.IsValid)
            {
                await _digitalPackageService.CreateAsync(digitalPackage, packageFile, previewFile);
                return RedirectToAction(nameof(Manage));
            }
            return View(digitalPackage);
        }

        // GET: Downloads/Edit/5
        [Authorize]
        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var digitalPackage = await _digitalPackageService.FindPackageAsync(id);
            if (digitalPackage == null)
            {
                return NotFound();
            }
            return View(digitalPackage);
        }

        // POST: Downloads/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, [Bind("ID,Title,Description,FileType")] DigitalPackage digitalPackage)
        {
            if (id != digitalPackage.ID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                if (await _digitalPackageService.EditPackageAsync(id, digitalPackage) == null)
                {
                    return NotFound();
                }

                return RedirectToAction(nameof(Manage));
            }
            return View(digitalPackage);
        }

        // GET: Downloads/Delete/5
        [Authorize]
        public async Task<IActionResult> Delete(Guid? id)
        {
            var digitalPackage = await _digitalPackageService.FindPackageAsync(id);
            if (digitalPackage == null)
            {
                return NotFound();
            }

            return View(digitalPackage);
        }

        // POST: Downloads/Delete/5
        [HttpPost, ActionName("Delete")]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            await _digitalPackageService.RemovePackageAsync(id);
            return RedirectToAction(nameof(Manage));
        }

    }
}
