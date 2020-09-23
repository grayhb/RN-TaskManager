using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using RN_TaskManager.Web.Services;

namespace RN_TaskManager.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger, IUserService userService)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View();
        }

  
    }
}
