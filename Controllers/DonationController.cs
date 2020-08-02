using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Assignment1.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Stripe;

namespace Assignment1.Controllers
{
    public class DonationController : Controller
    {
        private readonly GeorgianCollegeContext _context;

        private IConfiguration _configuration;

        public DonationController(GeorgianCollegeContext context, IConfiguration configuration)
        {
            _context = context;

            _configuration = configuration;
        }
        public IActionResult Index()
        {
            var Courses = _context.Courses.OrderBy(c => c.Name).ToList();
            return View(Courses);
        }

        public IActionResult Browse(string course)
        {

            ViewBag.Course = course;

            var Students = _context.Students.Where(p => p.Course.Name == course).OrderBy(p => p.Name).ToList();
            return View(Students);

        }

        public IActionResult StudentDetails(string Student)
        {
            var SelectedStudent = _context.Students.SingleOrDefault(p => p.Name == Student);
            return View(SelectedStudent);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]

        public IActionResult AddToCarts(int Credits, int StudentId)
        {
            var Student = _context.Students.SingleOrDefault(p => p.StudentId == StudentId);
            var price = Student.Price;

            var cartUsername = GetCartUserName();

            var cartItem = _context.Carts.SingleOrDefault(c => c.StudentId == StudentId && c.Username == cartUsername);
            if (cartItem == null)
            {
                var carts = new Carts
                {
                    StudentId = StudentId,
                    Credits = Credits,
                    Price = price,
                    Username = cartUsername
                };
                _context.Carts.Add(carts);

            }   
            else
                {
                cartItem.Credits += Credits;
                _context.Update(cartItem);
                }

            

            _context.SaveChanges();
            return RedirectToAction("Carts");
        }

        private string GetCartUserName()
        {

            if(HttpContext.Session.GetString("cartUsername") == null)
            {
                var cartUsername = "";
                    
                    if(User.Identity.IsAuthenticated)
                {
                    cartUsername = User.Identity.Name;
                }
                else
                {
                    cartUsername = Guid.NewGuid().ToString();
                }
                HttpContext.Session.SetString("CartUserName", cartUsername);
            }
            return HttpContext.Session.GetString("CartUserName");
        }

        public IActionResult Carts()

        { var cartUsername = GetCartUserName();
          var cartItems = _context.Carts.Include(c=> c.Students).Where(c => c.Username == cartUsername).ToList();
            return View(cartItems);  
                }

        public IActionResult RemoveFromCarts(int id)
        {
            var cartsItem = _context.Carts.SingleOrDefault(c => c.CartId == id);

            _context.Carts.Remove(cartsItem);
            _context.SaveChanges();
            return RedirectToAction("Carts");
        }

        public IActionResult Checkout()
        {
            MigrateCart();
            return View();
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Checkout([Bind("FirstName, LastName, Address,City,Province,PostalCode,Phone")] Models.Donations donations)
        {

            donations.DonationDate = DateTime.Now;
            donations.UserId = User.Identity.Name;

            var cartItems = _context.Carts.Where(c => c.Username == User.Identity.Name);
            decimal cartTotal = (from c in cartItems
                                 select c.Credits * c.Price).Sum();

            donations.Total = cartTotal;

            // HttpContext.Session.SetString("cartTotal", cartTotal.ToString());
            HttpContext.Session.SetObject("Donations", donations);


            return RedirectToAction("Payment");
        }

        
        private void MigrateCart()
        {
            if(HttpContext.Session.GetString("CartUsername") != User.Identity.Name)
            {
                var cartUsername = HttpContext.Session.GetString("CartUsername");
                var cartItems = _context.Carts.Where(c => c.Username == cartUsername);

            foreach(var item in cartItems)
                {
                    item.Username = User.Identity.Name;
                    _context.Update(item);
                }
                _context.SaveChanges();

                HttpContext.Session.SetString("CartUsername", User.Identity.Name);
            }
        }
        [Authorize]
        public IActionResult Payment()
        {

            var donations = HttpContext.Session.GetObject<Models.Donations>("Donations");

            ViewBag.Total = donations.Total;
            ViewBag.CentsTotal = donations.Total * 100;
            ViewBag.PublishableKey = _configuration.GetSection("Stripe")["PublishableKey"];

            return View();
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Payment(string stripeEmail, string stripeToken)
        {
            StripeConfiguration.ApiKey = _configuration.GetSection("Stripe")["SecretKey"];

            var cartUsername = HttpContext.Session.GetString("CartUsername");
            var cartItems = _context.Carts.Where(c => c.Username == cartUsername);
            var donations = HttpContext.Session.GetObject<Models.Donations>("Donations");

            var customerService = new CustomerService();
            var chargeService = new ChargeService();
            var customer = customerService.Create(new CustomerCreateOptions
            {
                Email = stripeEmail,
                Source = stripeToken

            });

            var charge = chargeService.Create(new ChargeCreateOptions
            {
                Amount = Convert.ToInt32(donations.Total * 100),
                Description = "Georgian College Donations",
                Currency = "cad",
                Customer = customer.Id
            });

            _context.Donations.Add(donations);
            _context.SaveChanges();

            foreach (var item in cartItems)
            {
                var donationDetails = new DonationDetails
                {
                    DonationId = donations.DonationId,
                    StudentId = item.StudentId,
                    Credits = item.Credits,
                    Price = item.Price
                };

                _context.DonationDetails.Add(donationDetails);
            }

            _context.SaveChanges();

            foreach(var item in cartItems)
            {
                _context.Carts.Remove(item);
            }
            _context.SaveChanges();



            return RedirectToAction("Details", "Donations", new { id = donations.DonationId});
        }

    }
}
