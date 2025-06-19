using BookIt1.Areas.Identity.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
using BookIt1.Models;
using BookIt1.Data;
using System;

[Authorize]
public class CartController : Controller
{
    private readonly BookIt1DbContext _context;
    private readonly UserManager<BookIt1User> _userManager;

    public CartController(BookIt1DbContext context, UserManager<BookIt1User> userManager)
    {
        _context = context;
        _userManager = userManager;
    }

    // View the cart for the logged-in user
    public async Task<IActionResult> Index()
    {
        var userId = _userManager.GetUserId(User);
        var cartItems = await _context.CartItems
            .Include(c => c.Event)
            .Where(c => c.UserId == userId)
            .ToListAsync();

        return View(cartItems);
    }

    // Add an item to the cart
    [HttpPost]
    public async Task<IActionResult> AddToCart(int eventId, int quantity)
    {
        if (quantity < 1)
            return BadRequest("Quantity must be at least 1.");

        var userId = _userManager.GetUserId(User);
        var eventItem = await _context.Events.FindAsync(eventId);
        if (eventItem == null)
            return NotFound();

        if (quantity > eventItem.AvailableTickets)
            return BadRequest("Requested quantity exceeds available seats.");

        var cartItem = await _context.CartItems
            .FirstOrDefaultAsync(c => c.UserId == userId && c.EventId == eventId);

        if (cartItem == null)
        {
            cartItem = new CartItem
            {
                UserId = userId,
                EventId = eventId,
                Quantity = quantity
            };
            _context.CartItems.Add(cartItem);
        }
        else
        {
            int newQuantity = cartItem.Quantity + quantity;
            if (newQuantity > eventItem.AvailableTickets)
                return BadRequest("Total quantity in cart exceeds available seats.");

            cartItem.Quantity = newQuantity;
            _context.CartItems.Update(cartItem);
        }

        await _context.SaveChangesAsync();
        return RedirectToAction("Index");
    }

    // Remove an item from the cart
    [HttpPost]
    public async Task<IActionResult> RemoveFromCart(int cartItemId)
    {
        var cartItem = await _context.CartItems.FindAsync(cartItemId);
        if (cartItem == null)
            return NotFound();

        _context.CartItems.Remove(cartItem);
        await _context.SaveChangesAsync();
        return RedirectToAction("Index");
    }

    // Update the quantity of an item in the cart
    [HttpPost]
    public async Task<IActionResult> UpdateCart(int cartItemId, int quantity)
    {
        if (quantity < 1)
            return BadRequest("Quantity must be at least 1.");

        var cartItem = await _context.CartItems
            .Include(c => c.Event)
            .FirstOrDefaultAsync(c => c.Id == cartItemId);

        if (cartItem == null)
            return NotFound();

        if (quantity > cartItem.Event.AvailableTickets)
            return BadRequest("Quantity exceeds available seats.");

        cartItem.Quantity = quantity;
        _context.CartItems.Update(cartItem);
        await _context.SaveChangesAsync();

        return RedirectToAction("Index");
    }

    // View checkout page
    public async Task<IActionResult> Checkout()
    {
        var userId = _userManager.GetUserId(User);
        var cartItems = await _context.CartItems
            .Include(c => c.Event)
            .Where(c => c.UserId == userId)
            .ToListAsync();

        if (!cartItems.Any())
            return RedirectToAction("Index");

        var total = cartItems.Sum(item => item.Quantity * item.Event.Price);

        var viewModel = new CheckoutViewModel
        {
            CartItems = cartItems,
            Total = total
        };

        return View(viewModel);
    }

    // Confirm checkout and place bookings
    [HttpPost]
    public async Task<IActionResult> ConfirmCheckout()
    {
        var userId = _userManager.GetUserId(User);
        var cartItems = await _context.CartItems
            .Include(c => c.Event)
            .Where(c => c.UserId == userId)
            .ToListAsync();

        if (!cartItems.Any())
            return RedirectToAction("Index");

        foreach (var item in cartItems)
        {
            if (item.Quantity > item.Event.AvailableTickets)
            {
                return BadRequest($"Not enough seats for event: {item.Event.Title}");
            }

            // Deduct seats
            item.Event.AvailableTickets -= item.Quantity;

            // Optional: create a booking record
            var booking = new Booking
            {
                UserId = userId,
                EventId = item.EventId,
                TicketsBooked = item.Quantity,
                BookingDate = DateTime.Now
            };
            _context.Bookings.Add(booking);
        }

        // Clear cart
        _context.CartItems.RemoveRange(cartItems);
        await _context.SaveChangesAsync();

        return RedirectToAction("ThankYou");
    }

    // Simple thank you page after checkout
    public IActionResult ThankYou()
    {
        return View();
    }

    public async Task<IActionResult> PaymentForm()
    {
        var userId = _userManager.GetUserId(User);
        var cartItems = await _context.CartItems
            .Include(c => c.Event)
            .Where(c => c.UserId == userId)
            .ToListAsync();

        if (!cartItems.Any()) return RedirectToAction("Index");

        var viewModel = new PaymentView
        {
            CartItems = cartItems,
            TotalAmount = cartItems.Sum(i => i.Event.Price * i.Quantity)
        };

        return View(viewModel);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> PaymentForm(PaymentView model)
    {
        var userId = _userManager.GetUserId(User);
        var cartItems = await _context.CartItems
            .Include(c => c.Event)
            .Where(c => c.UserId == userId)
            .ToListAsync();

        if (!cartItems.Any()) return RedirectToAction("Index");

        foreach (var item in cartItems)
        {
            if (item.Quantity > item.Event.AvailableTickets)
            {
                TempData["Error"] = $"Not enough seats for {item.Event.Title}.";
                return RedirectToAction("Index");
            }

            item.Event.AvailableTickets -= item.Quantity;

            var booking = new Booking
            {
                UserId = userId,
                EventId = item.EventId,
                TicketsBooked = item.Quantity,
                BookingDate = DateTime.Now,
                FullName = model.FullName,
                Email = model.Email,
                PhoneNumber = model.PhoneNumber,
                //Notes = model.Notes
            };

            _context.Bookings.Add(booking);
        }

        _context.CartItems.RemoveRange(cartItems);
        await _context.SaveChangesAsync();

        return RedirectToAction("PaymentSuccess");
    }
    public IActionResult PaymentSuccess()
    {
        return View();
    }

}

