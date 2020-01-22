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

        public async Task<IActionResult> Index() {         
            ViewData["SavedPosts"] = await _context.SavedPosts.Include(p => p.Post).ToListAsync();

            var appContext = _context.Posts.Include(p => p.User);
            return View(await appContext.ToListAsync());
        }

        public async Task<IActionResult> Offers() {
            ViewData["SavedPosts"] = await _context.SavedPosts.Include(p => p.Post).ToListAsync();

            var appContext = _context.Posts.Include(p => p.User).Where(p => p.PostType == PostType.Offer);
            return View(await appContext.ToListAsync());
        }

        public async Task<IActionResult> Requests() {
            ViewData["SavedPosts"] = await _context.SavedPosts.Include(p => p.Post).ToListAsync();

            var appContext = _context.Posts.Include(p => p.User).Where(p => p.PostType == PostType.Request);
            return View(await appContext.ToListAsync());
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
            if (post.UserId != userId) {
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

        public async Task<IActionResult> MyPosts() {

            var userId = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;

            var posts = _context.Posts.Where(x => x.UserId == userId).Include(p => p.User);
            ViewData["SavedPosts"] = await _context.SavedPosts.Include(p => p.Post).ToListAsync();
            return View(await posts.ToListAsync());
        }

        [Authorize]
        public async Task<IActionResult> ListSaved() {
            var userId = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId);

            var posts = _context.SavedPosts.Where(x => x.User == user).Include(p => p.Post).ThenInclude(p => p.User);
            return View(await posts.ToListAsync());
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