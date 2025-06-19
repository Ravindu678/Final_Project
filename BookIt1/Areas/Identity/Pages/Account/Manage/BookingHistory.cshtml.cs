using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using BookIt1.Data;
using BookIt1.Models;
using BookIt1.Areas.Identity.Data;

public class BookingHistoryModel : PageModel
{
    private readonly BookIt1DbContext _context;
    private readonly UserManager<BookIt1User> _userManager;

    public BookingHistoryModel(BookIt1DbContext context, UserManager<BookIt1User> userManager)
    {
        _context = context;
        _userManager = userManager;
    }

    public List<Booking> Bookings { get; set; }

    public async Task<IActionResult> OnGetAsync()
    {
        var userId = _userManager.GetUserId(User);

        Bookings = await _context.Bookings
            .Include(c => c.Event)
            .Where(c => c.UserId == userId)
            .ToListAsync();

        return Page();
    }

    public async Task<IActionResult> OnPostCancelAsync(int id)
    {
        var userId = _userManager.GetUserId(User);

        var booking = await _context.Bookings
            .Include(b => b.Event)
            .FirstOrDefaultAsync(b => b.Id == id && b.UserId == userId);

        if (booking == null)
        {
            return NotFound();
        }

        // Update available tickets
        booking.Event.AvailableTickets += booking.TicketsBooked;

        _context.Bookings.Remove(booking);
        await _context.SaveChangesAsync();

        return RedirectToPage(); // reload BookingHistory page
    }

}
