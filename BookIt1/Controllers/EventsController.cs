using Microsoft.AspNetCore.Mvc;

namespace BookIt1.Controllers
{
    public class EventsController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
