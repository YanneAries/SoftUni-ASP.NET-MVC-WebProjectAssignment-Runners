﻿using Microsoft.AspNetCore.Mvc;
using RunningWebApp.Interfaces;
using RunningWebApp.Models;
using RunningWebApp.ViewModels;

namespace RunningWebApp.Controllers
{
    public class ClubController : Controller
    {
        private readonly IClubRepository clubRepository;
        private readonly IPhotoService photoService;
        private readonly IHttpContextAccessor httpContextAccessor;

        public ClubController(IClubRepository clubRepository,
            IPhotoService photoService, IHttpContextAccessor httpContextAccessor)
        {
            this.clubRepository = clubRepository;
            this.photoService = photoService;
            this.httpContextAccessor = httpContextAccessor;
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
            var currUserId = httpContextAccessor.HttpContext.User.GetUserId();
            var clubCreateVM = new ClubCreateViewModel { AppUserId = currUserId };
            return View(clubCreateVM);
        }

		[HttpPost]
		public async Task<IActionResult> Create(ClubCreateViewModel clubVM)
		{
			if (ModelState.IsValid)
			{
                var result = await photoService.AddPhotoAsync(clubVM.Image);

                var club = new Club
                {
                    Title = clubVM.Title,
                    Description = clubVM.Description,
                    Image = result.Url.ToString(),
                    ClubCategory = clubVM.ClubCategory,
                    AppUserId = clubVM.AppUserId,
                    Address = new Address
                    {
                        Street = clubVM.Address.Street,
                        City = clubVM.Address.City,
                        State = clubVM.Address.State
                    }
                };
				clubRepository.Add(club);
				return RedirectToAction("Index");
			}
            else
            {
                ModelState.AddModelError("", "Photo upload failed");
            }

            return View(clubVM);
		}

		public async Task<IActionResult> Edit(int id)
        {
            var club = await clubRepository.GetByIdAsync(id);
            if (club == null) return View("Error");
            var clubEVM = new ClubEditViewModel
            {
                Title = club.Title,
                Description = club.Description,
                AddressId = (int)club.AddressId,
                Address = club.Address,
                URL = club.Image,
                ClubCategory = club.ClubCategory,
                AppUserId = club.AppUserId
            };
            return View(clubEVM);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(int id, ClubEditViewModel clubEVM)
        {
            if (!ModelState.IsValid)
            {
                ModelState.AddModelError("", "Failed to edit club");
                return View("Edit", clubEVM);
            }

            var userClub = await clubRepository.GetByIdAsyncNoTracking(id);

            if (userClub != null)
            {
                try
                {
					var img = new FileInfo(userClub.Image);
					var publicId = Path.GetFileNameWithoutExtension(img.Name);
					await photoService.DeletePhotoAsync(publicId);
				}
                catch (Exception e)
                {
                    ModelState.AddModelError("", "Could not delete photo");
                    return View(clubEVM);
                }

				var photoResult = await photoService.AddPhotoAsync(clubEVM.Image);
				var club = new Club
				{
					Id = id,
					Title = clubEVM.Title,
					Description = clubEVM.Description,
					Image = photoResult.Url.ToString(),
					ClubCategory = clubEVM.ClubCategory,
					AddressId = clubEVM.AddressId,
					Address = clubEVM.Address,
                    AppUserId = clubEVM.AppUserId
                };

				clubRepository.Update(club);
				return RedirectToAction("Index");
			}
            else
            {
                return View(clubEVM);
            }
        }

        public async Task<IActionResult> Delete(int id)
        {
            var clubDetails = await clubRepository.GetByIdAsync(id);
            if (clubDetails == null) { return View("Error"); }
            return View(clubDetails);
        }

		[HttpPost, ActionName("Delete")]
		public async Task<IActionResult> DeleteClub(int id)
		{
			var clubDetails = await clubRepository.GetByIdAsync(id);
            //Deleting pic from cloudinary
			var img = new FileInfo(clubDetails.Image);
			var publicId = Path.GetFileNameWithoutExtension(img.Name);
			await photoService.DeletePhotoAsync(publicId);

			if (clubDetails == null) return View("Error");

			clubRepository.Delete(clubDetails);
			return RedirectToAction("Index");
		}
	}
}
