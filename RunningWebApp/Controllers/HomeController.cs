using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using RunningWebApp.Helpers;
using RunningWebApp.Interfaces;
using RunningWebApp.Models;
using RunningWebApp.ViewModels;
using System.Diagnostics;
using System.Globalization;

namespace RunningWebApp.Controllers
{
    public class HomeController : Controller
    {
        private readonly IClubRepository clubRepository;

        public HomeController(IClubRepository clubRepository)
        {
            this.clubRepository = clubRepository;
        }

        public async Task<IActionResult> Index()
        {
            var ipInfo = new IPInfo();
            var homeVM = new HomeViewModel();
            try
            {
                string url = "https://ipinfo.io?token="; // secret ipInfo token goes here
                var info = new HttpClient().GetStringAsync(url).Result; //WebClient().DownloadString(url);
                ipInfo = JsonConvert.DeserializeObject<IPInfo>(info);
                RegionInfo myRI = new RegionInfo(ipInfo.Country);
                ipInfo.Country = myRI.EnglishName;
                homeVM.City = ipInfo.City;
                homeVM.State = ipInfo.Region;
                if (homeVM.City != null)
                {
                    homeVM.Clubs = await clubRepository.GetClubByCity(homeVM.City);
                }
                else
                {
                    homeVM.Clubs = null;
                }
                return View(homeVM);
            }
            catch (Exception)
            {
                homeVM.Clubs = null;
            }
            return View(homeVM);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });

            //if (statusCode == 404)
            //{
            //    return View("Error404");
            //}
        }
    }
}