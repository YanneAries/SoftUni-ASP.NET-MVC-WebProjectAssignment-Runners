using Microsoft.AspNetCore.Mvc;
using RunningWebApp.Data;

namespace RunningWebApp.Controllers
{
    public class RaceController : Controller
    {
		private readonly ApplicationDbContext context;

		public RaceController(ApplicationDbContext context)
		{
			this.context = context;
		}
		public IActionResult Index()
        {
			var races = context.Races.ToList();
            return View(races);
        }
    }
}
