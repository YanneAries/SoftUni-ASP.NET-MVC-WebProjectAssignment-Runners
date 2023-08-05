using CloudinaryDotNet.Actions;
using Microsoft.AspNetCore.Mvc;
using RunningWebApp.Interfaces;
using RunningWebApp.Models;
using RunningWebApp.ViewModels;

namespace RunningWebApp.Controllers
{
    public class DashboardController : Controller
	{
		private readonly IDashboardRepository dashboardRepository;
        private readonly IHttpContextAccessor httpContextAccessor;
        private readonly IPhotoService photoService;

        public DashboardController(IDashboardRepository dashboardRepository,
            IHttpContextAccessor httpContextAccessor, IPhotoService photoService)
        {
			this.dashboardRepository = dashboardRepository;
            this.httpContextAccessor = httpContextAccessor;
            this.photoService = photoService;
        }

        private void MapUserEdit(AppUser user, UserEditDashboardViewModel editVM, ImageUploadResult photoResult)
        {
            user.Id = editVM.Id;
            user.Pace = editVM.Pace;
            user.Mileage = editVM.Mileage;
            user.ProfileImageUrl = photoResult.Url.ToString();
            user.City = editVM.City;
            user.State = editVM.State;
        }

        public async Task<IActionResult> Index()
        {
            var userRaces = await dashboardRepository.GetAllUserRaces();
            var userClubs = await dashboardRepository.GetAllUserClubs();
            var dashboardViewModel = new DashboardViewModel()
            {
                Races = userRaces,
                Clubs = userClubs
            };
            return View(dashboardViewModel);
        }

        public async Task<IActionResult> EditUserProfile()
        {
            var currUserId = httpContextAccessor.HttpContext.User.GetUserId();
            var user = await dashboardRepository.GetUserById(currUserId);
            if (user == null) return View("Error");
            var userEditVM = new UserEditDashboardViewModel()
            {
                Id = currUserId,
                Pace = user.Pace,
                Mileage = user.Mileage,
                ProfileImageUrl = user.ProfileImageUrl,
                City = user.City,
                State = user.State,
            };
            return View(userEditVM);
        }

        [HttpPost]
        public async Task<IActionResult> EditUserProfile(UserEditDashboardViewModel editVM)
        {
            if (!ModelState.IsValid)
            {
                ModelState.AddModelError("", "Failed to edit profile");
                return View("EditUserProfile", editVM);
            }

            var user = await dashboardRepository.GetByIdNoTracking(editVM.Id);
            if (user.ProfileImageUrl == "" || user.ProfileImageUrl == null)
            {
                var photoResult = await photoService.AddPhotoAsync(editVM.Image);
                MapUserEdit(user, editVM ,photoResult); // done to avoid tracking errors
                dashboardRepository.Update(user);
                return RedirectToAction("Index");
            }
            else
            {
                try
                {
                    await photoService.DeletePhotoAsync(user.ProfileImageUrl);
                }
                catch (Exception)
                {
                    ModelState.AddModelError("", "Could not delete photo");
                    return View(editVM);
                }

                var photoResult = await photoService.AddPhotoAsync(editVM.Image);
                MapUserEdit(user, editVM, photoResult);
                dashboardRepository.Update(user);
                return RedirectToAction("Index");
            }
        }
    }
}
