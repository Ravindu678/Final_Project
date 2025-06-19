using System;
using System.IO;
using System.Linq;
using System.Text;
using BookIt1.Data;

namespace BookIt1.Services
{
    public class BookingDataExporter
    {

        //new code
        private readonly BookIt1DbContext _context;
        private readonly IWebHostEnvironment _env;

        public BookingDataExporter(BookIt1DbContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }

        public void UpdateBookingsCsv()
        {
            string filePath = Path.Combine(_env.WebRootPath, "data", "bookings.csv");
            ExportToCsv(filePath);
        }

        private void ExportToCsv(string filePath)
        {
            var bookings = _context.Bookings
                .Select(b => new
                {
                    b.EventId,
                    b.UserId,
                    b.BookingDate,
                    b.TicketsBooked,
                    b.FullName
                })
                .ToList();

            var csvLines = new List<string>
        {
            "EventId,UserId,BookingDate,TicketsBooked,FullName"
        };

            foreach (var b in bookings)
            {
                csvLines.Add($"{b.EventId},{b.UserId},{b.BookingDate:yyyy-MM-dd},{b.TicketsBooked},{b.FullName}");
            }

            var directory = Path.GetDirectoryName(filePath);
            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }

            File.WriteAllLines(filePath, csvLines);
        }
    }
}

