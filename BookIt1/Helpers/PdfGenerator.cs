using PdfSharpCore.Drawing;
using PdfSharpCore.Pdf;
using System;
using System.Collections.Generic;
using System.IO;

namespace BookIt1.Helpers
{
    public static class PdfGenerator
    {
        public static byte[] GenerateBookingSummary(
            string fullName,
            List<(string EventTitle, string Description, DateTime EventDate, string Location, int Quantity, decimal Price)> bookings)
        {
            var document = new PdfDocument();
            var page = document.AddPage();
            var gfx = XGraphics.FromPdfPage(page);

            var titleFont = new XFont("Helvetica", 18, XFontStyle.Bold);
            var subTitleFont = new XFont("Helvetica", 12, XFontStyle.BoldItalic);
            var headerFont = new XFont("Helvetica", 11, XFontStyle.Bold);
            var bodyFont = new XFont("Helvetica", 10, XFontStyle.Regular);
            var descFont = new XFont("Helvetica", 9, XFontStyle.Italic);

            var blackBrush = XBrushes.Black;
            var grayBrush = XBrushes.DarkGray;
            var headerBrush = new XSolidBrush(XColor.FromArgb(64, 64, 64));
            var highlightBrush = new XSolidBrush(XColor.FromArgb(0, 120, 215));
            var linePen = new XPen(XColors.LightGray, 0.5);

            int yPoint = 40;

            // Header
            gfx.DrawString("BOOKING CONFIRMATION", titleFont, highlightBrush, new XRect(0, yPoint, page.Width, 0), XStringFormats.TopCenter);
            yPoint += 30;
            gfx.DrawString($"Customer: {fullName}", subTitleFont, grayBrush, new XPoint(40, yPoint));
            yPoint += 30;

            // Table Headers
            gfx.DrawString("Event", headerFont, headerBrush, 40, yPoint);
            gfx.DrawString("Date", headerFont, headerBrush, 180, yPoint);
            gfx.DrawString("Location", headerFont, headerBrush, 270, yPoint);
            gfx.DrawString("Qty", headerFont, headerBrush, 390, yPoint);
            gfx.DrawString("Total (LKR)", headerFont, headerBrush, 460, yPoint);
            yPoint += 18;
            //gfx.DrawLine(linePen, 40, yPoint, 550, yPoint);
            yPoint += 10;

            decimal grandTotal = 0;

            foreach (var booking in bookings)
            {
                decimal total = booking.Price * booking.Quantity;
                grandTotal += total;

                gfx.DrawString(booking.EventTitle, bodyFont, blackBrush, 40, yPoint);
                gfx.DrawString(booking.EventDate.ToString("yyyy-MM-dd"), bodyFont, blackBrush, 180, yPoint);
                gfx.DrawString(booking.Location, bodyFont, blackBrush, 270, yPoint);
                gfx.DrawString(booking.Quantity.ToString(), bodyFont, blackBrush, 390, yPoint);
                gfx.DrawString($"Rs. {total:N2}", bodyFont, blackBrush, 460, yPoint);
                yPoint += 18;

                gfx.DrawString($"• {booking.Description}", descFont, grayBrush, 45, yPoint);
                yPoint += 20;

                gfx.DrawLine(new XPen(XColors.LightGray, 0.3), 40, yPoint, 550, yPoint);
                yPoint += 10;
            }

            // Grand Total
            yPoint += 15;
            gfx.DrawLine(new XPen(XColors.Black, 0.6), 350, yPoint, 550, yPoint);
            yPoint += 5;
            gfx.DrawString("Grand Total:", headerFont, highlightBrush, 360, yPoint);
            gfx.DrawString($"Rs. {grandTotal:N2}", headerFont, highlightBrush, 460, yPoint);

            // Footer
            yPoint += 40;
            gfx.DrawLine(new XPen(XColors.Silver, 0.4), 40, yPoint, 550, yPoint);
            yPoint += 20;
            gfx.DrawString("Thank you for booking with BookIt!", subTitleFont, new XSolidBrush(XColor.FromArgb(34, 139, 34)), new XRect(0, yPoint, page.Width, 0), XStringFormats.TopCenter);

            using var stream = new MemoryStream();
            document.Save(stream, false);
            return stream.ToArray();
        }
    }
}






