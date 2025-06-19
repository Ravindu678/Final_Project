using BookIt1.Areas.Identity.Data;
using BookIt1.Data;
using BookIt1.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;


namespace BookIt1.Controllers
{
    [Authorize]
    public class BookingController : Controller
    {
        private readonly BookIt1DbContext _context;
        //private readonly UserManager<IdentityUser> _userManager;
        private readonly UserManager<BookIt1User> _userManager;

        //public BookingController(BookIt1DbContext context, UserManager<IdentityUser> userManager)
       // {
        //    _context = context;
         //   _userManager = userManager;
       // }
        public BookingController(BookIt1DbContext context, UserManager<BookIt1User> userManager)
        {
            _context = context;
            _userManager = userManager;
        }


        public async Task<IActionResult> Booking(int id)
        {
            var ev = await _context.Events.FindAsync(id);
            if (ev == null) return NotFound();

            var vm = new BookingViewModel
            {
                EventId = ev.Id,
                EventTitle = ev.Title,
                AvailableTickets = ev.AvailableTickets
            };
            //return View(vm);
            //return View("Bookings", vm);
            return View("~/Views/Booking/Booking.cshtml", vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Booking(BookingViewModel model)
        {
            var ev = await _context.Events.FindAsync(model.EventId);
            if (ev == null) return NotFound();

            if (model.TicketsToBook <= 0 || model.TicketsToBook > ev.AvailableTickets)
            {
                ModelState.AddModelError("", "Invalid seat number.");
                return View(model);
            }

            var userId = _userManager.GetUserId(User);

            var Bookings = new Booking
            {
                EventId = ev.Id,
                UserId = userId,
                TicketsBooked = model.TicketsToBook,
                BookingDate = DateTime.Now,
                FullName = model.FullName,
                Email = model.Email,
                PhoneNumber = model.PhoneNumber,
                //Notes = model.Notes
                //PhoneNumber = model?.PhoneNumber ?? "N/A"
            };

            ev.AvailableTickets -= model.TicketsToBook;

            _context.Bookings.Add(Bookings);
            _context.Events.Update(ev);
            await _context.SaveChangesAsync();

            return RedirectToAction("BookingSuccess");
        }

        public IActionResult BookingSuccess()
        {
            return View();
        }
    }
}
