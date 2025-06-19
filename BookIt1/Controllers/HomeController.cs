using BookIt1.Areas.Admin.Models;
using BookIt1.Data;
using BookIt1.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using System.Net;
using System.Net.Mail;

namespace BookIt1.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly BookIt1DbContext _context;

        public HomeController(ILogger<HomeController> logger, BookIt1DbContext context)
        {
            _logger = logger;
            _context = context;
        }

        private async Task<List<Event>> GetOrderedEventsAsync()
        {
            return await _context.Events
                .OrderBy(e => e.Title)
                .ToListAsync();
        }

        public async Task<IActionResult> Index()
        {
            var events = await GetOrderedEventsAsync();
            return View(events); // Pass event list to Index.cshtml
        }

        public async Task<IActionResult> Events(string searchString)
        {
            var eventsQuery = _context.Events.AsQueryable();

            if (!string.IsNullOrEmpty(searchString))
            {
                eventsQuery = eventsQuery.Where(e =>
                    e.Title.Contains(searchString) ||
                    e.Category.Contains(searchString));
            }

            var events = await eventsQuery.OrderBy(e => e.Title).ToListAsync();
            return View(events); // For Events.cshtml
        }
        public async Task<IActionResult> EventDetails(int id)
        {
            var evt = await _context.Events.FindAsync(id);
            if (evt == null)
            {
                return NotFound();
            }

            return View(evt); // Return view with event details
        }


        public IActionResult Login()
        {
            return View();
        }
        public IActionResult About()
        {
            return View();
        }
        [HttpGet]
        public IActionResult Contact()
        {
            return View();
        }
        [HttpPost]
        public IActionResult Contact(SendEmail sendEmail)
        {
            if(!ModelState.IsValid) return View();

            try
            {
                MailMessage mail = new MailMessage();
                mail.From = new MailAddress("email");
                mail.To.Add("email");

                mail.Subject = sendEmail.Subject;

                mail.IsBodyHtml = true;
                string content = "Name:" + sendEmail.Name;
                content += "<br/> Email: " + sendEmail.Email;
                content += "<br/> Mobile: " + sendEmail.Mobile;
                content += "<br/> Message: " + sendEmail.Message;

                mail.Body = content;

                SmtpClient smtpClient = new SmtpClient("smtp.gmail.com");

                NetworkCredential networkCredential = new NetworkCredential("email", "app password");
                smtpClient.UseDefaultCredentials = false;
                smtpClient.Credentials = networkCredential;
                smtpClient.Port = 587;
                smtpClient.EnableSsl = true;
                smtpClient.Send(mail);

                ViewBag.Message = "Mail sent";

                ModelState.Clear();

            }
            catch (Exception ex)
            {

                ViewBag.Message = ex.Message.ToString();
            }

            return View();
        }
        public IActionResult Signup()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
        
    }
}
