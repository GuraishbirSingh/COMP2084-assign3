using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Assignment1.Models;
using Microsoft.AspNetCore.Authorization;

namespace Assignment1.Controllers
{
    [Authorize]
    public class DonationsController : Controller
    {
        private readonly GeorgianCollegeContext _context;

        public DonationsController(GeorgianCollegeContext context)
        {
            _context = context;
        }

        // GET: Donations
        public async Task<IActionResult> Index()
        {
            var username = User.Identity.Name;

            return View(await _context.Donations.Where(o => o.UserId == username).ToListAsync());
        }

        // GET: Donations/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            var username = User.Identity.Name;
            if (id == null)
            {
                return NotFound();
            }

            var donations = await _context.Donations.Where(o => o.UserId == username)
                .FirstOrDefaultAsync(m => m.DonationId == id);
            if (donations == null)
            {
                return NotFound();
            }

            return View(donations);
        }

        private bool DonationsExists(int id)
        {
            return _context.Donations.Any(e => e.DonationId == id);
        }
    }
}
