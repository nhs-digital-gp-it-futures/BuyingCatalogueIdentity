using System;
using System.Diagnostics;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using IdentityModel.Client;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using NHSD.BuyingCatalogue.Identity.Api.SampleMvcClient.ViewModels;

namespace NHSD.BuyingCatalogue.Identity.Api.SampleMvcClient.Controllers
{
    public sealed class HomeController : Controller
    {
        private readonly IConfiguration _configuration;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ILogger<HomeController> _logger;

        public HomeController(
            IConfiguration configuration, 
            IHttpClientFactory httpClientFactory,
            ILogger<HomeController> logger)
        {
            _configuration = configuration;
            _httpClientFactory = httpClientFactory;
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
                ViewBag.Response = "Authorized With Sample Resource";
            }

            return View();
        }

        [Authorize]
        public async Task<IActionResult> UserInfo()
        {
            var client = _httpClientFactory.CreateClient("IdentityClient");

            DiscoveryDocumentRequest discoveryDocumentRequest = new DiscoveryDocumentRequest
            {
                Policy = new DiscoveryPolicy
                {
                    RequireHttps = false
                }
            };

            var metaDataResponse = await client.GetDiscoveryDocumentAsync(discoveryDocumentRequest);
            if (metaDataResponse.IsError)
            {
                throw new Exception($"DiscoveryDocumentError : {metaDataResponse.Error}");
            }

            var accessToken = await HttpContext.GetTokenAsync(OpenIdConnectParameterNames.AccessToken);
            var userInfoResponse = await client.GetUserInfoAsync(new UserInfoRequest
            {
                Address = metaDataResponse.UserInfoEndpoint, 
                Token = accessToken
            });

            if (userInfoResponse.IsError)
            {
                throw new Exception($"UserInfoResponse : {userInfoResponse.Error}");
            }

            return View(new UserInfoViewModel { UserClaims = userInfoResponse.Claims });
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            await HttpContext.SignOutAsync(OpenIdConnectDefaults.AuthenticationScheme);

            var homeUrl = Url.Action(nameof(Index), "Home");
            return SignOut(new AuthenticationProperties { RedirectUri = homeUrl }, OpenIdConnectDefaults.AuthenticationScheme);
        }
    }
}
