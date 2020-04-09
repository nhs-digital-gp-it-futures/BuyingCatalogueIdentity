using System;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NHSD.BuyingCatalogue.Organisations.Api.Models;
using NHSD.BuyingCatalogue.Organisations.Api.ViewModels.Organisations;

namespace NHSD.BuyingCatalogue.Organisations.Api.Controllers
{
    [Authorize(Policy = Policy.CanAccessOrganisation)]
    [Route("api/v1/ods")]
    [ApiController]
    [Produces("application/json")]
    public sealed class OdsController : Controller
    {
        [HttpGet]
        [Route("{odsCode}")]
        public ActionResult GetByOdsCodeAsync(string odsCode)
        {
            if (odsCode is null)
                throw new ArgumentNullException(nameof(odsCode));

            if (string.IsNullOrWhiteSpace(odsCode))
                return NotFound();

            // Canned data
            return Ok(new OdsViewModel
            {
                OrganisationName = "Canned Organisation",
                OdsCode = odsCode,
                PrimaryRoleId = "123",
                Address = new AddressViewModel
                {
                    Line1 = "294  Charmaine Lane",
                    Line2 = "Amarillo",
                    Line3 = "Texas",
                    Town = "Amarillo",
                    County = "Texas",
                    Postcode = "73E8",
                    Country = "USA",
                }
            });
        }
    }
}
