using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RunningWebApp.Data;
using RunningWebApp.Interfaces;
using RunningWebApp.Models;
using RunningWebApp.Repository;
using RunningWebApp.Services;
using RunningWebApp.ViewModels;

namespace RunningWebApp.Controllers
{
    public class RaceController : Controller
    {
		private readonly IRaceRepository raceRepository;
		private readonly IPhotoService photoService;
		private readonly IHttpContextAccessor httpContextAccessor;

		public RaceController(IRaceRepository raceRepository, IPhotoService photoService, IHttpContextAccessor httpContextAccessor)
		{
			this.raceRepository = raceRepository;
			this.photoService = photoService;
			this.httpContextAccessor = httpContextAccessor;
		}

		public async Task<IActionResult> Index()
        {
			var races = await raceRepository.GetAll();
            return View(races);
        }

        public async Task<IActionResult> Detail(int id)
        {
            Race race = await raceRepository.GetByIdAsync(id);
            return View(race);
        }

		public IActionResult Create()
		{
            var currUserId = httpContextAccessor.HttpContext?.User.GetUserId();
            var raceCreateVM = new RaceCreateViewModel { AppUserId = currUserId };
            return View(raceCreateVM);
        }

		[HttpPost]
		public async Task<IActionResult> Create(RaceCreateViewModel raceVM)
		{
			if (ModelState.IsValid)
			{
				var result = await photoService.AddPhotoAsync(raceVM.Image);

				var race = new Race
				{
					Title = raceVM.Title,
					Description = raceVM.Description,
					Image = result.Url.ToString(),
					RaceCategory = raceVM.RaceCategory,
                    AppUserId = raceVM.AppUserId,
                    Address = new Address
					{
						Street = raceVM.Address.Street,
						City = raceVM.Address.City,
						State = raceVM.Address.State
					}
				};
				raceRepository.Add(race);
				return RedirectToAction("Index");
			}
			else
			{
				ModelState.AddModelError("", "Photo upload failed");
			}

			return View(raceVM);
		}

		public async Task<IActionResult> Edit(int id)
		{
			var race = await raceRepository.GetByIdAsync(id);
			if (race == null) return View("Error");
			var raceEVM = new RaceEditViewModel
			{
				Title = race.Title,
				Description = race.Description,
				AddressId = (int)race.AddressId,
				Address = race.Address,
				URL = race.Image,
				RaceCategory = race.RaceCategory,
            };
			return View(raceEVM);
		}

		[HttpPost]
		public async Task<IActionResult> Edit(int id, RaceEditViewModel raceEVM)
		{
			if (!ModelState.IsValid)
			{
				ModelState.AddModelError("", "Failed to edit race");
				return View("Edit", raceEVM);
			}

			var userRace = await raceRepository.GetByIdAsyncNoTracking(id);

			if (userRace != null)
			{
				try
				{
					var img = new FileInfo(userRace.Image);
					var publicId = Path.GetFileNameWithoutExtension(img.Name);
					await photoService.DeletePhotoAsync(publicId);
				}
				catch (Exception e)
				{
					ModelState.AddModelError("", "Could not delete photo");
					return View(raceEVM);
				}

				var photoResult = await photoService.AddPhotoAsync(raceEVM.Image);
				var race = new Race
				{
					Id = id,
					Title = raceEVM.Title,
					Description = raceEVM.Description,
					Image = photoResult.Url.ToString(),
					RaceCategory = raceEVM.RaceCategory,
					AddressId = raceEVM.AddressId,
					Address = raceEVM.Address,
                    //AppUserId = raceEVM.AppUserId
                };

				raceRepository.Update(race);
				return RedirectToAction("Index");
			}
			else
			{
				return View(raceEVM);
			}
		}

		public async Task<IActionResult> Delete(int id)
		{
			var raceDetails = await raceRepository.GetByIdAsync(id);
			if (raceDetails == null) return View("Error");
			return View(raceDetails);
		}

		[HttpPost, ActionName("Delete")]
		public async Task<IActionResult> DeleteRace(int id)
		{
			var raceDetails = await raceRepository.GetByIdAsync(id);
			//Deleting pic from cloudinary
			var img = new FileInfo(raceDetails.Image);
			var publicId = Path.GetFileNameWithoutExtension(img.Name);
			await photoService.DeletePhotoAsync(publicId);

			if (raceDetails == null) return View("Error");

			raceRepository.Delete(raceDetails);
			return RedirectToAction("Index");
		}
	}
}
