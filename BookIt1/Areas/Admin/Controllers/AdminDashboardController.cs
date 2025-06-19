using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using BookIt1.Models.ViewModels;
using BookIt1.Data;
//using Microsoft.EntityFrameworkCore;

[Authorize(Roles = "Admin")]
[Area("Admin")]
public class AdminDashboardController : Controller
{
    private readonly BookIt1DbContext _context;

    public AdminDashboardController(BookIt1DbContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> IndexAsync()
    {
        int eventCount = await _context.Events.CountAsync();
        int userCount = await _context.Users.CountAsync();
        int totalBookings = await _context.Bookings.CountAsync();
        decimal totalSales = await _context.Bookings
        .Include(b => b.Event)
        .SumAsync(b => b.Event.Price);

        ViewBag.EventCount = eventCount;
        ViewBag.UserCount = userCount;
        ViewBag.TotalBookings = totalBookings;
        ViewBag.TotalSales = totalSales;


        return View();
    }

    

    public IActionResult AllBookings()
    {


        var bookings = _context.CartItems
            .Include(c => c.Event)
       
            .Select(c => new BookingSummary
            {
               
                EventTitle = c.Event.Title,
                Quantity = c.Quantity,
                PricePerTicket = c.Event.Price
            })
            .ToList();

        return View(bookings);
    }



}
