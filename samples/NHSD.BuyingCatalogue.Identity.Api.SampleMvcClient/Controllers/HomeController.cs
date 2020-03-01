using System;
using System.Diagnostics;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using NHSD.BuyingCatalogue.Identity.Api.SampleMvcClient.Models;

namespace NHSD.BuyingCatalogue.Identity.Api.SampleMvcClient.Controllers
{
    public sealed class HomeController : Controller
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<HomeController> _logger;

        public HomeController(IConfiguration configuration, ILogger<HomeController> logger)
        {
            _configuration = configuration;
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View();
        }

        [Authorize]
        public async Task<IActionResult> Privacy()
        {
            var accessToken = await HttpContext.GetTokenAsync("access_token");

            // call api
            var apiClient = new HttpClient();

            apiClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
            var address = _configuration.GetSection("sampleResourceUrl");
            var response = await apiClient.GetAsync(address.Value);
            if (!response.IsSuccessStatusCode)
            {
                ViewBag.Response = response.ReasonPhrase;
                Console.WriteLine(response.StatusCode);
            }
            else
            {
                var content = await response.Content.ReadAsStringAsync();
                Console.WriteLine(JArray.Parse(content));
                ViewBag.Response = JArray.Parse(content);
            }
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        public async Task<IActionResult> Logout()
        {
            _logger.LogInformation("Start Logging out user from client");

            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            await HttpContext.SignOutAsync(OpenIdConnectDefaults.AuthenticationScheme);

            _logger.LogInformation("Stop Logging out user from client");

            var homeUrl = Url.Action(nameof(Index), "Home");
            return new SignOutResult(OpenIdConnectDefaults.AuthenticationScheme, new AuthenticationProperties { RedirectUri = homeUrl });
        }
    }
}
