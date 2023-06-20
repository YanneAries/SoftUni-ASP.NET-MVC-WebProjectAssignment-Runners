using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RunningWebApp.Data;
using RunningWebApp.Models;

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

        public IActionResult Detail(int id)
        {
            Race race = context.Races.Include(a => a.Address).FirstOrDefault(r => r.Id == id);
            return View(race);
        }
    }
}
