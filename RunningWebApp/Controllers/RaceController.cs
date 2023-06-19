using Microsoft.AspNetCore.Mvc;

namespace RunningWebApp.Controllers
{
    public class RaceController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
