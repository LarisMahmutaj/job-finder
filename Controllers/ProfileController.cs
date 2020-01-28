using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using JobFinder.Data;
using JobFinder.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace JobFinder.Controllers
{
   
    public class ProfileController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public ProfileController(IHttpContextAccessor httpContextAccessor, ApplicationDbContext context)
        {
            _httpContextAccessor = httpContextAccessor;
            _context = context;
        }

        public async Task<IActionResult> Index(string id)
        {
            //var userProfile = await _context.UserProfiles.Include(x => x.User).FirstOrDefaultAsync(u => u.UserId == userId);
            if (id == null) {
                return NotFound();
            }
            var userProfile = await _context.UserProfiles.Include(x => x.User).Where(p => p.UserId == id).FirstOrDefaultAsync();
            
            var userId = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;
            ViewData["LoggedUserId"] = userId;

            ViewData["ProfileUserId"] = id;

            return View((UserProfile)userProfile);         
        }   

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> Edit(int? id)
        {       
            if(id == null)
            {
                return NotFound();
            }
            var userProfile = await _context.UserProfiles.FindAsync(id);
            if(userProfile == null)
            {
                return NotFound();
            }

            var userId = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;
            if(userProfile.UserId != userId)
            {
                return Forbid();
            }
            return View(userProfile);
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, UserProfile userProfile)
        {
            if(id != userProfile.Id)
            {
                return NotFound();
            }
            var userId = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId);
            userProfile.User = user;

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(userProfile);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!UserProfileExists(userProfile.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }

            return View(userProfile);
            
        }

       [Authorize]
        public async Task<IActionResult> Create(UserProfile userProfile)
        {
            var userId = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId);
            userProfile.User = user;

            if (ModelState.IsValid)
            {
                _context.Add(userProfile);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(userProfile);
        }
        private bool UserProfileExists(int id)
        {
            return _context.UserProfiles.Any(x => x.Id == id);
        }
    }

    
}