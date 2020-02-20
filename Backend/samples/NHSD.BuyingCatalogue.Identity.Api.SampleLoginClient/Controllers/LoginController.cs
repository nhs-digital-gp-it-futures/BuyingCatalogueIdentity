using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

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
        public IActionResult Index(string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;
            return View("Index");
        }

        [HttpPost]
        public async Task<IActionResult> Index(string username, string password, string returnUrl)
        {
            // discover endpoints from metadata
            var client = new HttpClient();
            var putObject = JsonConvert.SerializeObject(new {username, password, returnUrl});
            var response = await client.PostAsync("http://localhost:52598/api/Authenticate/Login"
                , new StringContent(putObject.ToString(), Encoding.UTF8, "application/json"));
            

            if (!response.IsSuccessStatusCode)
            {
                return Problem("We're all doomed!");
            }
            
            foreach (var header in response.Headers)
            {
                if (header.Key == "Set-Cookie")
                {
                    foreach (var cKey in header.Value)
                    {
                        var parts = cKey.Split('=');
                        Response.Cookies.Append(parts[0], parts[1]);
                    }
                }
            }
            return Redirect("http://localhost:52598/api/Authenticate/Login");
        }
    }
}
