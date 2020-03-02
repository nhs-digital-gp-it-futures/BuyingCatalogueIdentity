﻿using System;
using System.Diagnostics;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json.Linq;
using NHSD.BuyingCatalogue.Identity.Api.SampleMvcClient.Models;

namespace NHSD.BuyingCatalogue.Identity.Api.SampleMvcClient.Controllers
{
    public sealed class HomeController : Controller
    {
        private readonly IConfiguration _configuration;

        public HomeController(IConfiguration configuration)
        {
            _configuration = configuration;
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

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
