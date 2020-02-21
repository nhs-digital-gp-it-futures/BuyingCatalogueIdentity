using System.Linq;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace NHSD.BuyingCatalogue.Identity.Api.SampleResource.Controllers
{
    [Route("identity")]
    [Authorize]
    public sealed class IdentityController : ControllerBase
    {
        [HttpGet]
        public IActionResult Get()
        {
            return new JsonResult(from c in User.Claims select new { c.Type, c.Value });
        }
    }
}
