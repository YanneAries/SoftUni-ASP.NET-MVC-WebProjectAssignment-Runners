using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RunningWebApp.Data;
using RunningWebApp.Interfaces;
using RunningWebApp.Models;
using RunningWebApp.Repository;

namespace RunningWebApp.Controllers
{
    public class ClubController : Controller
    {
        private readonly IClubRepository clubRepository;

        public ClubController(IClubRepository clubRepository)
        {
            this.clubRepository = clubRepository;
        }

        public async Task<IActionResult> Index()
        {
            var clubs = await clubRepository.GetAll();
            return View(clubs);
        }

        public async Task<IActionResult> Detail(int id)
        {
            Club club = await clubRepository.GetByIdAsync(id);
            return View(club);
        }

        public IActionResult Create()
        {
            return View();
        }

		[HttpPost]
		public IActionResult Create(Club club)
		{
			if (!ModelState.IsValid)
			{
				return View(club);
			}
			clubRepository.Add(club);
			return RedirectToAction("Index");
		}
	}
}
