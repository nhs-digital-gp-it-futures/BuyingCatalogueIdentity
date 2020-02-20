using System.Net.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace NHSD.BuyingCatalogue.Identity.Api.SampleLoginClient.Controllers
{
    public class LoginController : Controller
    {
        private readonly ILogger<LoginController> _logger;

        public LoginController(ILogger<LoginController> logger)
        {
            _logger = logger;
        }

        [HttpGet]
        public IActionResult Index(string redirectUrl)
        {
            ViewBag.RedirectUrl = redirectUrl;
            return View("Index");
        }

        [HttpPost]
        public ActionResult Index(string username, string password, string redirectUrl)
        {
            //create http client
            using var client = new HttpClient();




            return View("Index");
        }
    }
}
