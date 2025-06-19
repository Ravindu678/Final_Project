/*using BookIt1.Areas.Identity.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
using BookIt1.Models;
using BookIt1.Data;
using System;

namespace BookIt1.Controllers
{
    public class CheckoutController : Controller
    {
        private readonly BookIt1DbContext _db; // Define DbContext (adjust class name if necessary)

        public CheckoutController(BookIt1DbContext _db)
        {
            _db = _db; // Inject DbContext
        }

        // GET: Checkout/Review
        public ActionResult Review()
        {
            var userName = User.Identity.Name; // Get the logged-in user's name
            var user = db.Users.FirstOrDefault(u => u.UserName == userName); // Fetch user from database

            // Get cart items (from session or database, adjust as needed)
            var cartItems = GetCartItemsFromSession(); // This method should fetch cart items
            //var total = cartItems.Sum(x => x.Price * x.Quantity); // Calculate total price
            var total = cartItems.Sum(x => x.Event.Price * x.Quantity);

            // Prepare model for Review page
            var model = new ReviewViewModel
            {
                Name = user?.Name, // User's name
                Email = user?.Email, // User's email
                CartItems = cartItems, // List of cart items
                TotalPrice = total // Total price of items
            };

            return View(model); // Pass the model to the view
        }

        // Helper method to get cart items from session (replace with actual logic)
        private List<CartItem> GetCartItemsFromSession()
        {
            // Implement logic to get cart items, could be from session or database
            return new List<CartItem>(); // Placeholder
        }
    }
}*/

/*using BookIt1.Areas.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Collections.Generic;
using BookIt1.Models;
using BookIt1.Data;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;
using System;
//using BookIt1.ViewModels; // <-- Make sure this is added

namespace BookIt1.Controllers
{
    public class CheckoutController : Controller
    {
        private readonly BookIt1DbContext _db;

        public CheckoutController(BookIt1DbContext db)
        {
            _db = db;
        }

        // GET: Checkout/Review
        public ActionResult Review()
        {
            var userName = User.Identity.Name;
            var user = _db.Users.FirstOrDefault(u => u.UserName == userName);

            if (user == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var cartItems = GetCartItemsFromSession();

            var total = cartItems.Sum(x => x.Event.Price * x.Quantity);

            var model = new ReviewViewModel
            {
                UserId = user.Id,                  // Ensure UserId is set
                Name = user != null ? $"{user.FirstName} {user.LastName}" : "",
                Email = user.Email,
                CartItems = cartItems,
                TotalPrice = total
            };

            return View(model);
        }

        // POST: Checkout/ConfirmBooking
        [HttpPost]
        public async Task<ActionResult> ConfirmBooking(ReviewViewModel model)
        {
            var booking = new Booking
            {
                UserId = model.UserId,
                TotalPrice = model.TotalPrice,
                BookingDate = DateTime.Now
            };

            _db.Bookings.Add(booking);
            await _db.SaveChangesAsync();

            // Optionally clear cart
            HttpContext.Session.Remove("Cart");

            return RedirectToAction("BookingSuccess");
        }

        // GET: Checkout/BookingSuccess
        public ActionResult BookingSuccess()
        {
            return View();
        }

        // Helper: Get Cart Items from Session
        private List<CartItem> GetCartItemsFromSession()
        {
            var cartJson = HttpContext.Session.GetString("Cart");

            return string.IsNullOrEmpty(cartJson)
                ? new List<CartItem>()
                : JsonConvert.DeserializeObject<List<CartItem>>(cartJson);
        }
    }
}*/

/*using BookIt1.Data;
using BookIt1.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace BookIt1.Controllers
{
    public class CheckoutController : Controller
    {
        private readonly BookIt1DbContext _db;
        private readonly UserManager<BookIt1.Areas.Identity.Data.BookIt1User> _userManager;

        public CheckoutController(BookIt1DbContext db, UserManager<BookIt1.Areas.Identity.Data.BookIt1User> userManager)
        {
            _db = db;
            _userManager = userManager;
        }

        private List<CartItem> GetCartItemsFromSession()
        {
            var cartJson = HttpContext.Session.GetString("Cart");

            if (string.IsNullOrEmpty(cartJson))
                return new List<CartItem>();

            var cartItems = JsonConvert.DeserializeObject<List<CartItem>>(cartJson);

            // Rehydrate event details from DB
            foreach (var item in cartItems)
            {
                item.Event = _db.Events.FirstOrDefault(e => e.Id == item.EventId);
            }

            return cartItems;
        }

        /*public async Task<IActionResult> Review()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
                return RedirectToAction("Login", "Account");

            var cartItems = GetCartItemsFromSession();

            var totalPrice = cartItems.Sum(i => i.Event?.Price * i.Quantity ?? 0);

            var viewModel = new ReviewViewModel
            {
                UserId = user.Id,
                Name = user.FirstName + " " + user.LastName,
                Email = user.Email,
                CartItems = cartItems,
                TotalPrice = totalPrice
            };

            return View(viewModel);
        }*/

        /*[HttpPost]
        public IActionResult ConfirmBooking(ReviewViewModel model)
        {
            // You can add logic to save the booking to database here

            // Clear the cart after booking
            HttpContext.Session.Remove("Cart");

            return RedirectToAction("Success");
        }

        public IActionResult Success()
        {
            return View();
        }
    }
}*/


