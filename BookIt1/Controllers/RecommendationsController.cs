using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using BookIt1.Models;
using BookIt1.Services.Recommendation;
using BookIt1.Areas.Identity.Data;

namespace BookIt1.Controllers
{
    [Authorize]

    [Route("Home/[controller]")]
        public class RecommendationsController : Controller
        {
            private readonly RecommendationDataService _recommendationService;
            private readonly UserManager<BookIt1User> _userManager;

            public RecommendationsController(RecommendationDataService recommendationService, UserManager<BookIt1User> userManager)
            {
                _recommendationService = recommendationService;
                _userManager = userManager;
            }

            public async Task<IActionResult> Index()
            {
                var user = await _userManager.GetUserAsync(User);
                if (user == null) return Challenge();

                var recommendedEvents = _recommendationService.GetRecommendedEventsFromCsv(user.Id);
                return View(recommendedEvents);
            }
        }
    }

