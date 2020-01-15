using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using JobFinder.Data;
using JobFinder.Models;
using Microsoft.AspNetCore.Mvc;

namespace JobFinder.Controllers
{
    public class UsersController : Controller
    {
        private readonly ApplicationDbContext _context;

        public UsersController(ApplicationDbContext context) {
            _context = context;
        }
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public ViewResult Register() {
            return View();
        }

        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> Register([Bind("Id, Email, Username, Password, DateRegistered")] User user) {
        //    if (ModelState.IsValid) {
        //        _context.Add(user);
        //        await _context.SaveChangesAsync();
        //        return RedirectToAction(nameof(Index))
        //    }
        //}
    }
}