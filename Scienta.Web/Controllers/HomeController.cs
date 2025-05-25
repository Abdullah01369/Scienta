using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Scienta.Services.IServices;
using Scienta.Web.Models;

namespace Scienta.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IArticleService _articleService;

        public HomeController(ILogger<HomeController> logger,IArticleService articleService)
        {
              _articleService = articleService;
            _logger = logger;
        }

        public async Task<IActionResult> Index()
        {

      var evrimagacilist= await   _articleService.GetEvrimAgaciArticles(1);
        // var val=  await  _articleService.GetPopSciArticles(1);
        return View(evrimagacilist);
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
    }
}
