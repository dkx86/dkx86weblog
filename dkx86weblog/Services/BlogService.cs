using dkx86weblog.Data;
using dkx86weblog.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace dkx86weblog.Services
{
    public class BlogService
    {
        private readonly ApplicationDbContext _context;
        
        public BlogService(ApplicationDbContext context)
        {
            _context = context;
        }

        internal async Task<BlogViewModel> GetPublishedPostsAsync(int page)
        {
            var itemsAll = _context.Post.Where(p => p.Published).OrderByDescending(p => p.CreateTime);
            return await MakeViewModel(itemsAll, page);
        }

        internal async Task<BlogViewModel> GetAllPostsAsync(int page)
        {
            var itemsAll = _context.Post.OrderByDescending(p => p.CreateTime);
            return await MakeViewModel(itemsAll, page);
        }

        private async Task<BlogViewModel> MakeViewModel(IQueryable<Post> itemsAll, int page)
        {
            var itemsCount = await itemsAll.CountAsync();
            var itemsForPage = await itemsAll.Skip((page - 1) * PageViewModel.PAGE_SIZE).Take(PageViewModel.PAGE_SIZE).ToListAsync();

            PageViewModel pageModel = new PageViewModel(itemsCount, page);
            return new BlogViewModel(itemsForPage, pageModel);
        }

        internal async Task CreatePostAsync(Post post)
        {
            post.ID = Guid.NewGuid();
            post.CreateTime = DateTime.Now;
            post.UpdateTime = post.CreateTime;

            _context.Add(post);
            await _context.SaveChangesAsync();
        }

        internal async Task<List<Post>> ListBlogForRssAsync(int itemsCount)
        {
            return await _context.Post.Where(p => p.Published).OrderByDescending(p => p.CreateTime).Take(itemsCount).ToListAsync();
        }

        internal bool PostExists(Guid id)
        {
            return _context.Post.Any(e => e.ID == id);
        }

        internal async Task<Post> FindPostAsync(Guid? id)
        {
            return await _context.Post.FirstOrDefaultAsync(m => m.ID == id);
        }

        internal async Task RemovePostAsync(Guid id)
        {
            var post = await FindPostAsync(id);
            if (post == null)
                return;

            _context.Post.Remove(post);
            await _context.SaveChangesAsync();
        }

        internal async Task SwitchPublishStateAsync(Guid? postId, bool publish)
        {
            var post = await FindPostAsync(postId);
            if (post == null)
                return;
            if (publish)
            {
                post.CreateTime = DateTime.Now;
                post.UpdateTime = post.CreateTime;
            }

            post.Published = publish;
            await _context.SaveChangesAsync();
        }

        internal async Task<Post> EditPostAsync(Guid id, Post updatedPost)
        {
            var post = await FindPostAsync(id);
            try
            {
                post.UpdateTime = DateTime.Now;
                post.Body = updatedPost.Body;
                post.Title = updatedPost.Title;

                _context.Update(post);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                return null;
            }
            return post;
        }
    }
}


