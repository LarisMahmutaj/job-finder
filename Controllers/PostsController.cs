using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using JobFinder.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Authorization;
using JobFinder.Data;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using System.Net;

namespace JobFinder.Controllers {
    public class PostsController : Controller {

        private readonly ApplicationDbContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public PostsController(IHttpContextAccessor httpContextAccessor, ApplicationDbContext context) {
            _context = context;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<IActionResult> Index(int? pageIndex, string search) {         
            ViewData["SavedPosts"] = await _context.SavedPosts.Include(p => p.Post).ToListAsync();

            List<Post> posts;

            if (search != null) {
                search.ToLower();
                posts = await _context.Posts
                    .Where(x => x.Title.ToLower().Contains(search) || x.Content.ToLower().Contains(search) || x.User.UserName.ToLower().Contains(search))
                    .Include(p => p.User)
                    .ToListAsync();
            } else {
                posts = await _context.Posts.Include(p => p.User).ToListAsync();
            }
         
            posts.Reverse();
        
            PaginatedList<Post> paginatedPosts = PaginatedList<Post>.Create(posts, pageIndex ?? 1, 5);

            return View(paginatedPosts);
        }

        public async Task<IActionResult> Offers(int? pageIndex, string search) {
            ViewData["SavedPosts"] = await _context.SavedPosts.Include(p => p.Post).ToListAsync();

            List<Post> posts;

            if (search != null) {
                search.ToLower();
                posts = await _context.Posts
                    .Where(x => x.Title.ToLower().Contains(search) || x.Content.ToLower().Contains(search) || x.User.UserName.ToLower().Contains(search))
                    .Include(p => p.User)
                    .ToListAsync();
            } else {
                posts = await _context.Posts.Include(p => p.User).ToListAsync();
            }

            posts.Reverse();

            PaginatedList<Post> paginatedPosts = PaginatedList<Post>.Create(posts, pageIndex ?? 1, 5);
            return View(paginatedPosts);
        }

        public async Task<IActionResult> Requests(int? pageIndex, string search) {
            ViewData["SavedPosts"] = await _context.SavedPosts.Include(p => p.Post).ToListAsync();

            List<Post> posts;

            if (search != null) {
                search.ToLower();
                posts = await _context.Posts
                    .Where(x => x.Title.ToLower().Contains(search) || x.Content.ToLower().Contains(search) || x.User.UserName.ToLower().Contains(search))
                    .Include(p => p.User)
                    .ToListAsync();
            } else {
                posts = await _context.Posts.Include(p => p.User).ToListAsync();
            }

            posts.Reverse();

            PaginatedList<Post> paginatedPosts = PaginatedList<Post>.Create(posts, pageIndex ?? 1, 5);
            return View(paginatedPosts);
        }

        [Authorize]
        public IActionResult Create() {

            ViewData["UserId"] = new SelectList(_context.Users, "Id", "UserName");
            return View();
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Post post) {
            post.DatePosted = DateTime.Now;
            var userId = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId);
            post.User = user;

            if (ModelState.IsValid) {
                _context.Add(post);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(post);
        }

        [Authorize]
        public async Task<IActionResult> Edit(int? id) {
            if (id == null) {
                return NotFound();
            }
            var post = await _context.Posts.FindAsync(id);
                if (post == null) {
                return NotFound();
            }
            
            var userId = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;
            if (post.UserId != userId){
                return Forbid();
            }
            return View(post);
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Post post) {
            if(id != post.Id) {
                return NotFound();
            }
            var userId = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId);
            post.User = user;

            if (ModelState.IsValid) {
                try {
                    _context.Update(post);
                    await _context.SaveChangesAsync();
                } catch (DbUpdateConcurrencyException) {
                    if (!PostExists(post.Id)) {
                        return NotFound();
                    } else {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(post);
        }

        [Authorize]
        public async Task<IActionResult> Delete(int? id) {
            if(id == null) {
                return NotFound();
            }

            var post = await _context.Posts.Include(p => p.User).FirstOrDefaultAsync(x => x.Id == id);
            if(post == null) {
                return NotFound();
            }

            var userId = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;
            if (post.UserId != userId && !User.IsInRole("Admin")) {
                return Forbid();
            }

            return View(post);
        }

        [Authorize]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ConfirmDelete(int id) {
            var post = await _context.Posts.FindAsync(id);
            _context.Posts.Remove(post);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> MyPosts(int? pageIndex) {

            var userId = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;

            ViewData["SavedPosts"] = await _context.SavedPosts.Include(p => p.Post).ToListAsync();

            var posts = await _context.Posts.Where(x => x.UserId == userId).Include(p => p.User).ToListAsync();
            posts.Reverse();
            PaginatedList<Post> paginatedPosts = PaginatedList<Post>.Create(posts, pageIndex ?? 1, 5);
            return View(paginatedPosts);
        }

        [Authorize]
        public async Task<IActionResult> ListSaved(int? pageIndex) {
            var userId = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId);

            var posts = await _context.SavedPosts.Where(x => x.User == user).Include(p => p.Post).ThenInclude(p => p.User).ToListAsync();
            posts.Reverse();

            PaginatedList<SavedPost> paginatedPosts = PaginatedList<SavedPost>.Create(posts, pageIndex ?? 1, 5);
            return View(paginatedPosts);
        }

        [Authorize]
        public async Task<IActionResult> SavePost(int? id) {
            if(id == null) {
                return NotFound();
            }

            var post = await _context.Posts.Include(p => p.User).FirstOrDefaultAsync(x => x.Id == id);

            if(post == null) {
                return NotFound();
            }

            SavedPost savedPost = new SavedPost();

            var userId = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId);

            savedPost.User = user;
            savedPost.Post = post;


            _context.SavedPosts.Add(savedPost);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        [Authorize]
        public async Task<IActionResult> UnsavePost(int? id) {
            if (id == null) {
                return NotFound();
            }

            var savedPosts = await _context.SavedPosts.Include(p => p.Post).ToListAsync();
            var userId = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;

            var savedPost = savedPosts.Where(x => x.PostId == id).Where(x => x.UserId == userId).First();
        
            if (savedPost == null) {
                return NotFound();
            }

            _context.SavedPosts.Remove(savedPost);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(ListSaved));
        }

        private bool PostExists(int id) {
            return _context.Posts.Any(p => p.Id == id);
        }
    }

}