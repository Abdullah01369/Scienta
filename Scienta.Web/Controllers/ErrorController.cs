using Microsoft.AspNetCore.Mvc;

namespace Scienta.Web.Controllers
{
    public class ErrorController : Controller
    {
        [Route("Error/{statusCode}")]
        public IActionResult ErrorPage(int statusCode)
        {
            if (statusCode == 404)
            {
                return View("NotFound");
            }

            return View("GenelHata");
        }
    }
}
