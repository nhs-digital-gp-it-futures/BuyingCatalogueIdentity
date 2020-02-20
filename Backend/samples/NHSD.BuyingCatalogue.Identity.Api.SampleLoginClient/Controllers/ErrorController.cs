using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace NHSD.BuyingCatalogue.Identity.Api.SampleLoginClient.Controllers
{
    public class ErrorController : Controller
    {
        private readonly ILogger<ErrorController> _logger;

        public ErrorController(ILogger<ErrorController> logger)
        {
            _logger = logger;
        }

        [HttpGet]
        public async  Task<IActionResult> Index(string errorId)
        {
            using var httpClient = new HttpClient();

            var res = await httpClient.GetAsync($"http://localhost:52598/api/Authenticate/Error?errorId={errorId}");

            return Ok(await res.Content.ReadAsStringAsync());
        }
    }
}
