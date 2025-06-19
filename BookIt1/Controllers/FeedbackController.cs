using Microsoft.AspNetCore.Mvc;

namespace BookIt1.Controllers
{
    public class FeedbackController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
