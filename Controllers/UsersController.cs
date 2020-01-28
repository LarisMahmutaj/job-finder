using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using JobFinder.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace JobFinder.Controllers {
    [Authorize(Policy = "RequireAdministratorRole")]
    public class UsersController : Controller {

        private readonly ApplicationDbContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public UsersController(IHttpContextAccessor httpContextAccessor, ApplicationDbContext context) {
            _context = context;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<IActionResult> Index() {
            var users = _context.Users;
            return View(await users.ToListAsync());
        }

        public async Task<IActionResult> LockoutUser(string id) {
            if (id == null) {
                return NotFound();
            }

            var user = _context.Users.Find(id);

            var date = DateTime.Now.AddMinutes(1);

            user.LockoutEnd = date;

            try {
                _context.Update(user);
                await _context.SaveChangesAsync();
            } catch (DbUpdateConcurrencyException) {
                throw;
            }

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> UnLockoutUser(string id) {
            if (id == null) {
                return NotFound();
            }

            var user = _context.Users.Find(id);


            user.LockoutEnd = null;

            try {
                _context.Update(user);
                await _context.SaveChangesAsync();
            } catch (DbUpdateConcurrencyException) {
                throw;
            }

            return RedirectToAction(nameof(Index));
        }
    }
}