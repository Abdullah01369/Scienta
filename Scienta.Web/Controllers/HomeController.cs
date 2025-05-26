using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Scienta.Services.IServices;
using Scienta.Services.Models;
using Scienta.Web.Models;

namespace Scienta.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IArticleService _articleService;

        public HomeController(ILogger<HomeController> logger, IArticleService articleService)
        {
            _articleService = articleService;
            _logger = logger;
        }

        public async Task<IActionResult> Index()
        {

            return View();
          
        }

        public async Task<IActionResult> EvrimAgaciArticles(int Id = 1)
        {
            if (Id < 1) {
                Id = 1;
            }
            ViewBag.BackId = Id-1;
            ViewBag.NextId = Id+1;
            ViewBag.Id = Id;
            var val = await _articleService.GetEvrimAgaciArticles(Id);
            return View(val);
        }

        public async Task<IActionResult> PopSciArticles(int Id = 1)
        {
            if (Id < 1)
            {
                Id = 1;
            }
            ViewBag.BackId = Id - 1;
            ViewBag.NextId = Id + 1;
            ViewBag.Id = Id;
            var val = await _articleService.GetPopSciArticles(Id);

            return View(val);
        }

        public async Task<IActionResult> BilimFiliArticles(int Id = 1)
        {
            if (Id < 1)
            {
                Id = 1;
            }
            ViewBag.BackId = Id - 1;
            ViewBag.NextId = Id + 1;
            ViewBag.Id = Id;
            var val = await _articleService.GetBilimFiliArticles(Id);

            return View(val);
        }


        public async Task<IActionResult> ReadArticle(string url)
        {
            string from="";
            ReadArticleModel model = new ReadArticleModel();
            if (url.Contains("popsci"))
            {
                from = PlatformsModel.GetPopsci();
            }
            if (url.Contains("evrimagaci"))
            {
                from = PlatformsModel.GetEvrimagaci();
            }
            if (url.Contains("bilimfili"))
            {
                from = PlatformsModel.GetBilimFili();
            }
            var val=await  _articleService.GetArticle(url,from);
            
            return View(val);
        }

    }
}
