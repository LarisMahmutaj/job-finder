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
    public class DashboardController : Controller {
        private readonly ApplicationDbContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public DashboardController(IHttpContextAccessor httpContextAccessor, ApplicationDbContext context) {
            _context = context;
            _httpContextAccessor = httpContextAccessor;
        }
        public async Task<IActionResult> Index() {
            ViewData["Users"] = await _context.Users.ToListAsync();
            ViewData["Posts"] = await _context.Posts.Include(p => p.User).ToListAsync();
            return View();
        }


    }
}
