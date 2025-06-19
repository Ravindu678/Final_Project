using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using BookIt1.Data; 
using BookIt1.Areas.Admin.Models.ViewModels;
using PdfSharpCore.Drawing;
using PdfSharpCore.Pdf;
using Microsoft.EntityFrameworkCore;
using PdfSharpCore;


namespace BookIt1.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class ReportsController : Controller
    {
        private readonly BookIt1DbContext _context;

        public ReportsController(BookIt1DbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult EventBookingReport()
        {
            var report = _context.Events
                .Select(e => new EventBookingReportViewModel
                {
                    EventName = e.Title,
                    EventDate = e.EventDate,
                    TotalTicketsSold = _context.Bookings
                        .Where(b => b.EventId == e.Id)
                        .Sum(b => (int?)b.TicketsBooked) ?? 0,

                    TotalRevenue = (_context.Bookings
                        .Where(b => b.EventId == e.Id)
                        .Sum(b => (int?)b.TicketsBooked) ?? 0) * e.Price,

                    UniqueBookers = _context.Bookings
                        .Where(b => b.EventId == e.Id)
                        .Select(b => b.UserId)
                        .Distinct()
                        .Count()
                })
                .ToList();

            return View(report);
        }

        [HttpPost]
        public IActionResult ExportEventBookingReport()
        {
            var reportData = _context.Events
                .Select(e => new EventBookingReportViewModel
                {
                    EventName = e.Title,
                    EventDate = e.EventDate,
                    TotalTicketsSold = _context.Bookings
                        .Where(b => b.EventId == e.Id)
                        .Sum(b => (int?)b.TicketsBooked) ?? 0,

                    TotalRevenue = (_context.Bookings
                        .Where(b => b.EventId == e.Id)
                        .Sum(b => (int?)b.TicketsBooked) ?? 0) * e.Price,

                    UniqueBookers = _context.Bookings
                        .Where(b => b.EventId == e.Id)
                        .Select(b => b.UserId)
                        .Distinct()
                        .Count()
                })
                .ToList();

            using (var stream = new MemoryStream())
            {
                var document = new PdfSharpCore.Pdf.PdfDocument();
                var page = document.AddPage();
                page.Size = PdfSharpCore.PageSize.A4;

                var gfx = PdfSharpCore.Drawing.XGraphics.FromPdfPage(page);
                var fontTitle = new XFont("Times New Roman", 16, XFontStyle.Bold);
                var fontHeader = new XFont("Times New Roman", 12, XFontStyle.Bold);
                var fontBody = new XFont("Times New Roman", 11, XFontStyle.Regular);


                double yPoint = 40;

                foreach (var item in reportData)
                {
                    gfx.DrawString($"{item.EventName} ({item.EventDate:dd MMM yyyy})", fontTitle, PdfSharpCore.Drawing.XBrushes.DarkBlue, new PdfSharpCore.Drawing.XRect(40, yPoint, page.Width - 80, 30), PdfSharpCore.Drawing.XStringFormats.TopLeft);
                    yPoint += 30;

                    gfx.DrawString("Total Tickets Sold:", fontHeader, PdfSharpCore.Drawing.XBrushes.Black, new PdfSharpCore.Drawing.XRect(60, yPoint, 200, 25), PdfSharpCore.Drawing.XStringFormats.TopLeft);
                    gfx.DrawString(item.TotalTicketsSold.ToString(), fontBody, PdfSharpCore.Drawing.XBrushes.Black, new PdfSharpCore.Drawing.XRect(260, yPoint, 200, 25), PdfSharpCore.Drawing.XStringFormats.TopLeft);
                    yPoint += 22;

                    gfx.DrawString("Unique Bookers:", fontHeader, PdfSharpCore.Drawing.XBrushes.Black, new PdfSharpCore.Drawing.XRect(60, yPoint, 200, 25), PdfSharpCore.Drawing.XStringFormats.TopLeft);
                    gfx.DrawString(item.UniqueBookers.ToString(), fontBody, PdfSharpCore.Drawing.XBrushes.Black, new PdfSharpCore.Drawing.XRect(260, yPoint, 200, 25), PdfSharpCore.Drawing.XStringFormats.TopLeft);
                    yPoint += 22;

                    gfx.DrawString("Total Revenue (LKR):", fontHeader, PdfSharpCore.Drawing.XBrushes.Black, new PdfSharpCore.Drawing.XRect(60, yPoint, 200, 25), PdfSharpCore.Drawing.XStringFormats.TopLeft);
                    gfx.DrawString(item.TotalRevenue.ToString("N2"), fontBody, PdfSharpCore.Drawing.XBrushes.DarkGreen, new PdfSharpCore.Drawing.XRect(260, yPoint, 200, 25), PdfSharpCore.Drawing.XStringFormats.TopLeft);
                    yPoint += 35;

                    yPoint += 20;

                    if (yPoint > page.Height - 100)
                    {
                        page = document.AddPage();
                        gfx = PdfSharpCore.Drawing.XGraphics.FromPdfPage(page);
                        yPoint = 40;
                    }
                }

                document.Save(stream, false);
                return File(stream.ToArray(), "application/pdf", "EventBookingReport.pdf");
            }
        }

        public IActionResult UserBookingSummary()
        {
            var users = _context.Users.ToList();

            var summary = users.Select(user => new UserBookingSummaryViewModel
            {
                FullName = user.FirstName, // adjust property name if different
                Email = user.Email,
                TotalBookings = _context.Bookings.Count(b => b.UserId == user.Id),
                TotalTickets = _context.Bookings
                                .Where(b => b.UserId == user.Id)
                                .Sum(b => (int?)b.TicketsBooked) ?? 0,
                TotalSpent = (from b in _context.Bookings
                              join e in _context.Events on b.EventId equals e.Id
                              where b.UserId == user.Id
                              select (decimal?)b.TicketsBooked * e.Price).Sum() ?? 0m

            }).Where(s => s.TotalBookings > 0) // only show users with bookings
              .ToList();

            return View(summary);
        }

        [HttpPost]
        public IActionResult ExportUserBookingSummary()
        {
            var data = _context.Users
                .Select(user => new UserBookingSummaryViewModel
                {
                    FullName = user.FirstName + " " + user.LastName,
                    Email = user.Email,
                    TotalBookings = _context.Bookings.Count(b => b.UserId == user.Id),
                    TotalTickets = _context.Bookings.Where(b => b.UserId == user.Id).Sum(b => (int?)b.TicketsBooked) ?? 0,
                    TotalSpent = (from b in _context.Bookings
                                  join e in _context.Events on b.EventId equals e.Id
                                  where b.UserId == user.Id
                                  select (decimal?)b.TicketsBooked * e.Price).Sum() ?? 0m
                })
                .Where(s => s.TotalBookings > 0)
                .ToList();

            using var stream = new MemoryStream();
            var document = new PdfDocument();
            var page = document.AddPage();
            page.Size = PdfSharpCore.PageSize.A4;

            var gfx = XGraphics.FromPdfPage(page);
            var fontTitle = new XFont("Times New Roman", 16, XFontStyle.Bold);
            var fontHeader = new XFont("Times New Roman", 12, XFontStyle.Bold);
            var fontBody = new XFont("Times New Roman", 11, XFontStyle.Regular);

            double yPoint = 40;

            gfx.DrawString("User Booking Summary", fontTitle, XBrushes.DarkBlue,
                new XRect(0, yPoint, page.Width, 30), XStringFormats.TopCenter);
            yPoint += 40;

            foreach (var user in data)
            {
                // Name + Email
                gfx.DrawString($"{user.FullName}", fontTitle, XBrushes.DarkBlue,
                    new XRect(40, yPoint, page.Width - 80, 30), XStringFormats.TopLeft);
                yPoint += 25;

                gfx.DrawString("Email:", fontHeader, XBrushes.Black, new XRect(60, yPoint, 200, 25), XStringFormats.TopLeft);
                gfx.DrawString(user.Email, fontBody, XBrushes.Black, new XRect(200, yPoint, 300, 25), XStringFormats.TopLeft);
                yPoint += 22;

                gfx.DrawString("Total Bookings:", fontHeader, XBrushes.Black, new XRect(60, yPoint, 200, 25), XStringFormats.TopLeft);
                gfx.DrawString(user.TotalBookings.ToString(), fontBody, XBrushes.Black, new XRect(200, yPoint, 300, 25), XStringFormats.TopLeft);
                yPoint += 22;

                gfx.DrawString("Total Tickets:", fontHeader, XBrushes.Black, new XRect(60, yPoint, 200, 25), XStringFormats.TopLeft);
                gfx.DrawString(user.TotalTickets.ToString(), fontBody, XBrushes.Black, new XRect(200, yPoint, 300, 25), XStringFormats.TopLeft);
                yPoint += 22;

                gfx.DrawString("Total Spent (LKR):", fontHeader, XBrushes.Black, new XRect(60, yPoint, 200, 25), XStringFormats.TopLeft);
                gfx.DrawString(user.TotalSpent.ToString("N2"), fontBody, XBrushes.DarkGreen, new XRect(200, yPoint, 300, 25), XStringFormats.TopLeft);
                yPoint += 35;

                yPoint += 15;

                // Add new page if needed
                if (yPoint > page.Height - 100)
                {
                    page = document.AddPage();
                    page.Size = PdfSharpCore.PageSize.A4;
                    gfx = XGraphics.FromPdfPage(page);
                    yPoint = 40;
                }
            }

            gfx.DrawString($"Generated on {DateTime.Now:yyyy-MM-dd HH:mm}", fontBody, XBrushes.Gray,
                new XRect(40, page.Height - 30, page.Width - 80, 20), XStringFormats.TopLeft);

            document.Save(stream, false);
            return File(stream.ToArray(), "application/pdf", "UserBookingSummaryReport.pdf");
        }


        public IActionResult DailySalesReport()
        {
            var dailyReport = _context.Bookings
                .Include(b => b.Event)
                .GroupBy(b => b.BookingDate.Date)
                .Select(g => new DailySalesReportViewModel
                {
                    Date = g.Key,
                    TotalTicketsSold = g.Sum(b => b.TicketsBooked),
                    TotalRevenue = g.Sum(b => b.TicketsBooked * b.Event.Price)
                })
                .OrderBy(r => r.Date)
                .ToList();

            return View(dailyReport);
        }


        [HttpPost]
        public IActionResult ExportDailySalesReport()
        {
            var reportData = _context.Bookings
                .Include(b => b.Event)
                .AsEnumerable()
                .GroupBy(b => b.BookingDate.Date)
                .Select(g => new
                {
                    Date = g.Key,
                    BookingsCount = g.Count(),
                    TotalTicketsSold = g.Sum(b => b.TicketsBooked),
                    TotalRevenue = g.Sum(b => b.TicketsBooked * b.Event.Price)
                })
                .OrderBy(r => r.Date)
                .ToList();

            using var stream = new MemoryStream();
            var document = new PdfDocument();
            var page = document.AddPage();
            page.Size = PageSize.A4;
            var gfx = XGraphics.FromPdfPage(page);
            var fontTitle = new XFont("Verdana", 14, XFontStyle.Bold);
            var fontHeader = new XFont("Verdana", 10, XFontStyle.Bold);
            var fontBody = new XFont("Verdana", 10, XFontStyle.Regular);
            double yPoint = 40;

            // Report Title
            gfx.DrawString("Daily Sales Revenue Report", fontTitle, XBrushes.DarkBlue,
                new XRect(0, yPoint, page.Width, 30), XStringFormats.TopCenter);
            yPoint += 40;

            foreach (var item in reportData)
            {
                gfx.DrawString($"Date: {item.Date:yyyy-MM-dd}", fontHeader, XBrushes.Black,
                    new XRect(40, yPoint, page.Width - 80, 25), XStringFormats.TopLeft);
                yPoint += 25;

                gfx.DrawString("Bookings Made:", fontHeader, XBrushes.Black, new XRect(60, yPoint, 200, 20), XStringFormats.TopLeft);
                gfx.DrawString(item.BookingsCount.ToString(), fontBody, XBrushes.Black, new XRect(200, yPoint, 200, 20), XStringFormats.TopLeft);
                yPoint += 20;

                gfx.DrawString("Tickets Sold:", fontHeader, XBrushes.Black, new XRect(60, yPoint, 200, 20), XStringFormats.TopLeft);
                gfx.DrawString(item.TotalTicketsSold.ToString(), fontBody, XBrushes.Black, new XRect(200, yPoint, 200, 20), XStringFormats.TopLeft);
                yPoint += 20;

                gfx.DrawString("Total Revenue (LKR):", fontHeader, XBrushes.Black, new XRect(60, yPoint, 200, 20), XStringFormats.TopLeft);
                gfx.DrawString(item.TotalRevenue.ToString("N2"), fontBody, XBrushes.DarkGreen, new XRect(200, yPoint, 200, 20), XStringFormats.TopLeft);
                yPoint += 30;

                // Page break
                if (yPoint > page.Height - 100)
                {
                    page = document.AddPage();
                    gfx = XGraphics.FromPdfPage(page);
                    yPoint = 40;
                }
            }

            // Footer
            gfx.DrawString($"Generated on {DateTime.Now:yyyy-MM-dd HH:mm}", fontBody, XBrushes.Gray,
                new XRect(40, page.Height - 30, page.Width - 80, 20), XStringFormats.CenterLeft);

            document.Save(stream, false);
            return File(stream.ToArray(), "application/pdf", "DailySalesRevenueReport.pdf");
        }

    }
}