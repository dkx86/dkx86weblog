using dkx86weblog.Data;
using dkx86weblog.Models;
using dkx86weblog.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace dkx86weblog.Controllers
{
    public class BlogController : Controller
    {
        private readonly BlogService _service;

        public BlogController(BlogService service)
        {
            _service = service;
        }

        // GET: Blog
        public async Task<IActionResult> Index(int page = 1)
        {
            var posts =  await _service.GetPublishedPostsAsync(page);
            return View(posts);
        }

        // GET: Blog/Post/5
        public async Task<IActionResult> Post(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var post = await _service.FindPostAsync(id);
            if (post == null)
            {
                return NotFound();
            }

            return View(post);
        }


        // GET: Blog/Manage
        [Authorize]
        public async Task<IActionResult> Manage(int page = 1)
        {
            var posts = await _service.GetAllPostsAsync(page);
            return View(posts);
        }

        // GET: Blog/Create
        [Authorize]
        public IActionResult Create()
        {
            return View();
        }

        // POST: Blog/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Title,Body")] Post post)
        {
            if (ModelState.IsValid)
            {
                await _service.CreatePostAsync(post);
                return RedirectToAction(nameof(Manage));
            }
            return View(post);
        }

        // GET: Blog/Edit/5
        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var post = await _service.FindPostAsync(id);
            if (post == null)
            {
                return NotFound();
            }
            return View(post);
        }

        // POST: Blog/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, [Bind("ID,Title,Body,Published")] Post post)
        {
            if (id != post.ID || !_service.PostExists(post.ID))
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                if(await _service.EditPostAsync(id, post) == null)
                    return NotFound();

                return RedirectToAction(nameof(Index));
            }
            return View(post);
        }

        // GET: Blog/Delete/5
        [Authorize]
        public async Task<IActionResult> Delete(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var post = await _service.FindPostAsync(id);
            if (post == null)
            {
                return NotFound();
            }

            return View(post);
        }

        // POST: Blog/Delete/5
        [Authorize]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            await _service.RemovePostAsync(id);
            return RedirectToAction(nameof(Manage));
        }

        // POST: Blog/Publish/5
        [Authorize]
        public async Task<IActionResult> Publish(Guid? id)
        {
            await _service.SwitchPublishStateAsync(id, true);
            
            return RedirectToAction(nameof(Manage));
        }

        // POST: Blog/Unpublish/5
        [Authorize]
        public async Task<IActionResult> Unpublish(Guid? id)
        {
            await _service.SwitchPublishStateAsync(id, false);

            return RedirectToAction(nameof(Manage));
        }


    }
}
