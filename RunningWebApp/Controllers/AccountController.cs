using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using RunningWebApp.Data;
using RunningWebApp.Models;
using RunningWebApp.ViewModels;

namespace RunningWebApp.Controllers
{
	public class AccountController : Controller
	{
		private readonly UserManager<AppUser> userManager;
		private readonly SignInManager<AppUser> signInManager;
		private readonly ApplicationDbContext context;

        public AccountController(UserManager<AppUser> userManager, SignInManager<AppUser> signInManager, ApplicationDbContext context)
        {
			this.context = context;
			this.userManager = userManager;
			this.signInManager = signInManager;
        }

        public IActionResult Login()
		{
			var response = new LoginViewModel();
			return View();
		}

		[HttpPost]
		public async Task<IActionResult> Login(LoginViewModel loginVM)
		{
			if(!ModelState.IsValid) return View(loginVM);

			var user = await userManager.FindByEmailAsync(loginVM.EmailAddress);

			if(user != null)
			{
				//User found
				var passwordCheck = await userManager.CheckPasswordAsync(user, loginVM.Password);
				if (passwordCheck)
				{
					var result = await signInManager.PasswordSignInAsync(user, loginVM.Password, false, false);
					if (result.Succeeded) { return  RedirectToAction("Index", "Race"); }
				}

				TempData["Error"] = "Wrong credentials. Please try again";
				return View(loginVM);
			}
			//User not found
			TempData["Error"] = "User not found";
			return View(loginVM);
		}
	}
}
