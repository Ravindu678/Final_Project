
using BookIt1.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Stripe.Checkout;
using BookIt1.Data;
using System.Security.Claims;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using System.Threading.Tasks;
using BookIt1.Areas.Identity.Data;
using BookIt1;
using BookIt1.Helpers;
using BookIt1.Services.Recommendation;
using BookIt1.Services;

public class PaymentController : Controller
{
    private readonly StripeSettings _stripeSettings;
    private readonly UserManager<BookIt1User> _userManager;
    private Booking booking;
    private readonly BookingDataExporter _bookingCsvService;
    private readonly RecommendationDataService _recommendationService;

    public PaymentController(
        IOptions<StripeSettings> stripeSettings,
        UserManager<BookIt1User> userManager,
        BookingDataExporter bookingCsvService,
        RecommendationDataService recommendationService)
    {
        _stripeSettings = stripeSettings.Value;
        _userManager = userManager;
        Stripe.StripeConfiguration.ApiKey = _stripeSettings.SecretKey;
        _bookingCsvService = bookingCsvService;
        _recommendationService = recommendationService;
    }

    [HttpPost]
    public IActionResult CreateCheckoutSession([FromServices] BookIt1DbContext _context)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var cartItems = _context.CartItems
            .Where(ci => ci.UserId == userId)
            .Include(ci => ci.Event)
            .ToList();

        var lineItems = new List<SessionLineItemOptions>();
        foreach (var item in cartItems)
        {
          
            var amountInLKR = (long)(item.Event.Price * 100);
            lineItems.Add(new SessionLineItemOptions
            {
                PriceData = new SessionLineItemPriceDataOptions
                {
                    Currency = "lkr",
                    UnitAmount = amountInLKR,
                    ProductData = new SessionLineItemPriceDataProductDataOptions
                    {
                        Name = item.Event.Title,
                    },
                },
                Quantity = item.Quantity,
            });
        }

        var options = new SessionCreateOptions
        {
            PaymentMethodTypes = new List<string> { "card" },
            LineItems = lineItems,
            Mode = "payment",
            SuccessUrl = "https://localhost:7195/Payment/Success",
            CancelUrl = "https://localhost:7195/Payment/Cancel",
        };

        var service = new SessionService();
        Session session = service.Create(options);
        return Redirect(session.Url);
    }

  

    public async Task<IActionResult> Success([FromServices] BookIt1DbContext _context)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var user = await _userManager.FindByIdAsync(userId);

        var cartItems = _context.CartItems
            .Where(c => c.UserId == userId)
            .Include(c => c.Event)
            .ToList();

        //var bookingSummary = new List<(string EventTitle, int Quantity, decimal Price)>();
        var bookingSummary = new List<(string EventTitle, string Description, DateTime EventDate, string Location, int Quantity, decimal Price)>();


        foreach (var item in cartItems)
        {
            // bookingSummary.Add((item.Event.Title, item.Quantity, item.Event.Price));

            bookingSummary.Add((
                item.Event.Title,
                item.Event.Description,
                item.Event.EventDate, // or item.Event.Date depending on your model
                item.Event.Location,
                item.Quantity,
                item.Event.Price
    ));

            var booking = new Booking
            {
                UserId = userId,
                EventId = item.EventId,
                TicketsBooked = item.Quantity,
                BookingDate = DateTime.Now,
                PhoneNumber = user?.PhoneNumber ?? "Not Provided",
                FullName = user?.FirstName ?? "No Name",
                Email = user?.Email ?? "No Email"
            };

            _context.Bookings.Add(booking);
            item.Event.AvailableTickets -= item.Quantity;
        }

        _context.CartItems.RemoveRange(cartItems);
        await _context.SaveChangesAsync();

        
        _bookingCsvService.UpdateBookingsCsv(); 
        _recommendationService.GenerateTrainingDataCsv();         

        // Generate PDF
        var pdfBytes = PdfGenerator.GenerateBookingSummary(user?.UserName ?? "Guest", bookingSummary);

        if (!string.IsNullOrEmpty(user?.Email))
        {
            await EmailSender.SendEmailWithPdfAsync(
                user.Email,
                "Your Booking Confirmation",
                "Thank you for your booking. Please find your booking summary attached.",
                pdfBytes
            );
        }

        return View(); // Views/Payment/Success.cshtml
    }


    public IActionResult Cancel()
    {
        return View(); // Shows Views/Payment/Cancel.cshtml
    }
}


