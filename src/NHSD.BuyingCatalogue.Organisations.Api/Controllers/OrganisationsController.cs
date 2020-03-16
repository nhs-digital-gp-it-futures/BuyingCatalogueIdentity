using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NHSD.BuyingCatalogue.Organisations.Api.Models;
using NHSD.BuyingCatalogue.Organisations.Api.Repositories;
using NHSD.BuyingCatalogue.Organisations.Api.ViewModels.Organisations;

namespace NHSD.BuyingCatalogue.Organisations.Api.Controllers
{
    [Authorize]
    [Route("api/v1/Organisations")]
    [ApiController]
    [Produces("application/json")]
    public sealed class OrganisationsController : Controller
    {
        private readonly IOrganisationRepository _organisationRepository;

        public OrganisationsController(IOrganisationRepository organisationRepository)
        {
            _organisationRepository = organisationRepository;
        }

        [HttpGet]
        public async Task<ActionResult> GetAllAsync()
        {
            IEnumerable<Organisation> organisationsList = await _organisationRepository.ListOrganisationsAsync();

            return Ok(new GetAllOrganisationsViewModel
            {
                Organisations = organisationsList?.Select(x =>
                    new OrganisationViewModel
                    {
                        OrganisationId = x.Id,
                        Name = x.Name,
                        OdsCode = x.OdsCode,
                        PrimaryRoleId = x.PrimaryRoleId,
                        CatalogueAgreementSigned = x.CatalogueAgreementSigned,
                        Location = x.Location == null ? null : new LocationViewModel
                        {
                            Line1 = x.Location.Line1,
                            Line2 = x.Location.Line2,
                            Line3 = x.Location.Line3,
                            Line4 = x.Location.Line4,
                            Town = x.Location.Town,
                            County = x.Location.County,
                            Postcode = x.Location.Postcode,
                            Country = x.Location.Country,
                        }
                    })
            });
        }

        [HttpGet]
        [Route("{id}")]
        public async Task<ActionResult> GetByIdAsync(Guid id)
        {
            var organisation = await _organisationRepository.GetByIdAsync(id);

            if (organisation is null)
            {
                return NotFound();
            }

            return Ok(new OrganisationViewModel
            {
                OrganisationId = organisation.Id,
                Name = organisation.Name,
                OdsCode = organisation.OdsCode,
                PrimaryRoleId = organisation.PrimaryRoleId,
                CatalogueAgreementSigned = organisation.CatalogueAgreementSigned,
                Location = organisation.Location == null ? null : new LocationViewModel
                {
                    Line1 = organisation.Location.Line1,
                    Line2 = organisation.Location.Line2,
                    Line3 = organisation.Location.Line3,
                    Line4 = organisation.Location.Line4,
                    Town = organisation.Location.Town,
                    County = organisation.Location.County,
                    Postcode = organisation.Location.Postcode,
                    Country = organisation.Location.Country,
                }
            });
        }
    }
}
