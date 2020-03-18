using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NHSD.BuyingCatalogue.Organisations.Api.Models;
using NHSD.BuyingCatalogue.Organisations.Api.Repositories;
using NHSD.BuyingCatalogue.Organisations.Api.ViewModels.Organisations;
using NHSD.BuyingCatalogue.Organisations.Api.ViewModels.OrganisationUsers;

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
                Organisations = organisationsList?.Select(organisation =>
                    new OrganisationViewModel
                    {
                        OrganisationId = organisation.OrganisationId,
                        Name = organisation.Name,
                        OdsCode = organisation.OdsCode,
                        PrimaryRoleId = organisation.PrimaryRoleId,
                        CatalogueAgreementSigned = organisation.CatalogueAgreementSigned,
                        Address = organisation.Address is null ? null : new AddressViewModel
                        {
                            Line1 = organisation.Address.Line1,
                            Line2 = organisation.Address.Line2,
                            Line3 = organisation.Address.Line3,
                            Line4 = organisation.Address.Line4,
                            Town = organisation.Address.Town,
                            County = organisation.Address.County,
                            Postcode = organisation.Address.Postcode,
                            Country = organisation.Address.Country,
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
                OrganisationId = organisation.OrganisationId,
                Name = organisation.Name,
                OdsCode = organisation.OdsCode,
                PrimaryRoleId = organisation.PrimaryRoleId,
                CatalogueAgreementSigned = organisation.CatalogueAgreementSigned,
                Address = organisation.Address is null ? null : new AddressViewModel
                {
                    Line1 = organisation.Address.Line1,
                    Line2 = organisation.Address.Line2,
                    Line3 = organisation.Address.Line3,
                    Line4 = organisation.Address.Line4,
                    Town = organisation.Address.Town,
                    County = organisation.Address.County,
                    Postcode = organisation.Address.Postcode,
                    Country = organisation.Address.Country,
                }
            });
        }
    }
}
