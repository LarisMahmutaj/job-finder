using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using JobFinder.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace JobFinder.Controllers {
    public class PostsController : Controller {
        private readonly JobFinderDbContext _context;

        public PostsController(JobFinderDbContext context) {
            _context = context;
        }

        public async Task<IActionResult> Index() {
            var jobFinderDbContext = _context.Posts.Include(p => p.User);
            return View(await jobFinderDbContext.ToListAsync());
        }

        public IActionResult Create() {
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "Username");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Post post) {
            post.DatePosted = DateTime.Now;
            if (ModelState.IsValid) {
                _context.Add(post);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "Username", post.UserId);
            return View(post);
        }

        public async Task<IActionResult> Delete(int? id) {
            if(id == null) {
                return NotFound();
            }

            var post = await _context.Posts.Include(p => p.User).FirstOrDefaultAsync(x => x.Id == id);
            if(post == null) {
                return NotFound();
            }

            return View(post);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ConfirmDelete(int id) {
            var post = await _context.Posts.FindAsync(id);
            _context.Posts.Remove(post);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool PostExists(int id) {
            return _context.Posts.Any(p => p.Id == id);
        }
    }

}
