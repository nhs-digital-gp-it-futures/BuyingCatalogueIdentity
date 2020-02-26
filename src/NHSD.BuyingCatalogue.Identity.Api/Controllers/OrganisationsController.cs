using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NHSD.BuyingCatalogue.Identity.Api.Models;
using NHSD.BuyingCatalogue.Identity.Api.Repositories;
using NHSD.BuyingCatalogue.Identity.Api.ViewModels.Organisations;

namespace NHSD.BuyingCatalogue.Identity.Api.Controllers
{
    [Route("api/v1/Organisations")]
    [ApiController]
    [Produces("application/json")]
    [AllowAnonymous]
    public sealed class OrganisationsController : ControllerBase
    {
        private readonly IOrganisationRepository _organisationRepository;

        public OrganisationsController(IOrganisationRepository organisationRepository)
        {
            _organisationRepository = organisationRepository;
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult> GetAll()
        {
            IEnumerable<Organisation> organisationModelList = await _organisationRepository.ListOrganisationsAsync();

            return Ok(new GetAllOrganisationsResponseViewModel
            {
                Organisations = organisationModelList.Select(x => new OrganisationViewModel
                {
                    OrganisationId = x.Id,
                    Name = x.Name,
                    OdsCode = x.OdsCode
                })
            });
        }
    }
}
