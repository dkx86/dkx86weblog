using dkx86weblog.Data;
using dkx86weblog.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace dkx86weblog.Services
{
    public class BlogService
    {

        protected readonly ApplicationDbContext _context;
        protected readonly ILogger<BlogService> _logger;

        public BlogService(ApplicationDbContext context, ILogger<BlogService> logger)
        {
            _logger = logger;
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

        internal async Task<Post> CreatePostAsync()
        {
            var curentTime = DateTime.Now;
            var post = new Post
            {
                ID = Guid.NewGuid(),
                CreateTime = curentTime,
                UpdateTime = curentTime
            };

            try
            {
                _context.Add(post);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException e)
            {
                _logger.LogError(e.Message, e);
            }
            return post;
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
            {
                _logger.LogError("Post {postId} not found!", id);
                return;
            }

            try
            {
                _context.Remove(post);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException e)
            {
                _logger.LogError(e.Message, e);
            }
        }

        internal async Task SwitchPublishStateAsync(Guid? postId, bool publish)
        {
            var post = await FindPostAsync(postId);
            if (post == null)
            {
                _logger.LogError("Post {postId} not found!", postId);
                return;
            }

            if (publish)
            {
                post.CreateTime = DateTime.Now;
                post.UpdateTime = post.CreateTime;
            }

            post.Published = publish;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException e)
            {
                _logger.LogError(e.Message, e);
            }
        }


        internal async Task<Post> EditPostAsync(Guid id, Post updatedPost)
        {
            var post = await FindPostAsync(id);
            post.UpdateTime = DateTime.Now;
            post.Body = updatedPost.Body;
            post.Title = updatedPost.Title;

            try
            {
                _context.Update(post);
                await _context.SaveChangesAsync();
                return post;
            }
            catch (DbUpdateConcurrencyException e)
            {
                _logger.LogError(e.Message, e);
                return null;
            }

        }
    }
}


