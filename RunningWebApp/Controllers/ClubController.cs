using Microsoft.AspNetCore.Mvc;
using RunningWebApp.Data;

namespace RunningWebApp.Controllers
{
    public class ClubController : Controller
    {
        private readonly ApplicationDbContext context;

        public ClubController(ApplicationDbContext context)
        {
            this.context = context;
        }

        public IActionResult Index()
        {
            var clubs = context.Clubs.ToList();
            return View(clubs);
        }
    }
}
