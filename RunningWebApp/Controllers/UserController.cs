using Microsoft.AspNetCore.Mvc;
using RunningWebApp.Interfaces;
using RunningWebApp.ViewModels;

namespace RunningWebApp.Controllers
{
    public class UserController : Controller
    {
        private readonly IUserRepository userRepository;
        public UserController(IUserRepository userRepository)
        {
            this.userRepository = userRepository;
        }

        [HttpGet("Users")]
        public async Task<IActionResult> Index()
        {
            var users = await userRepository.GetAllUsers();
            List<UserViewModel> usersList = new List<UserViewModel>();
            foreach (var user in users)
            {
                var userViewModel = new UserViewModel
                {
                    Id = user.Id,
                    UserName = user.UserName,
                    Pace = user.Pace,
                    Mileage = user.Mileage
                };
                usersList.Add(userViewModel);
            }
            return View(usersList);
        }

        public async Task<IActionResult> Detail(string id)
        {
            var user = await userRepository.GetUserById(id);
            var userDetailViewModel = new UserDetailViewModel
            {
                Id = user.Id,
                UserName = user.UserName,
                Pace = user.Pace,
                Mileage = user.Mileage
            };
            return View(userDetailViewModel);
        }
    }
}
