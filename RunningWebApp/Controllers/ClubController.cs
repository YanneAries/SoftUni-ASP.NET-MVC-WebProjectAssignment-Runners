using Microsoft.AspNetCore.Mvc;

namespace RunningWebApp.Controllers
{
    public class ClubController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
