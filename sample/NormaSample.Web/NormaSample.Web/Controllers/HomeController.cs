using System.Diagnostics;
using EC.Norma.Metadata;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using NormaSample.Web.Models;

namespace NormaSample.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> logger;

        public HomeController(ILogger<HomeController> logger)
        {
            this.logger = logger;
        }

        [ByPassNorma]
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        [ByPassNorma]
        public IActionResult AccessDenied()
        {
            // Here we will trace the atempt for auditory pourposes
            return View();
        }
    }
}
