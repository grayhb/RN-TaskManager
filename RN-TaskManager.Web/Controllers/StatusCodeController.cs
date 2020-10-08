using Microsoft.AspNetCore.Mvc;

namespace RN_TaskManager.Web.Controllers
{
    [Route("[controller]")]

    public class StatusCodeController : Controller
    {
        [HttpGet("403")]
        public IActionResult Page403()
        {
            return View();
        }

        [HttpGet("404")]
        public IActionResult Page404()
        {
            return View();
        }
    }
}
