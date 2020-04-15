using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NHSD.BuyingCatalogue.Identity.Common.Constants;
using NHSD.BuyingCatalogue.Organisations.Api.Models;
using NHSD.BuyingCatalogue.Organisations.Api.Repositories;
using NHSD.BuyingCatalogue.Organisations.Api.ViewModels.Organisations;

namespace NHSD.BuyingCatalogue.Organisations.Api.Controllers
{
    [Authorize(Policy = PolicyName.CanAccessOrganisations)]
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

        [Authorize(Policy = PolicyName.CanManageOrganisations)]
        [HttpPut]
        [Route("{id}")]
        public async Task<ActionResult> UpdateOrganisationByIdAsync(Guid id, UpdateOrganisationViewModel viewModel)
        {
            if (viewModel is null)
            {
                throw new ArgumentNullException(nameof(viewModel));
            }
            var organisation = await _organisationRepository.GetByIdAsync(id);

            if (organisation is null)
            {
                return NotFound();
            }

            organisation.CatalogueAgreementSigned = viewModel.CatalogueAgreementSigned;

            await _organisationRepository.UpdateAsync(organisation);

            return NoContent();
        }

        [Authorize(Policy = PolicyName.CanManageOrganisations)]
        [HttpPost]
        public async Task<ActionResult<CreateOrganisationResponseViewModel>> CreateOrganisationAsync(CreateOrganisationRequestViewModel viewModel)
        {
            if (viewModel is null)
            {
                throw new ArgumentNullException(nameof(viewModel));
            }

            //TODO: Task #6168

            // Canned data
            var firstOrganisation = (await _organisationRepository.ListOrganisationsAsync()).First();

            var response = new CreateOrganisationResponseViewModel
            {
                Errors = null,
                OrganisationId = firstOrganisation.OrganisationId.ToString()
            };

            return Created(new Uri($"/{response.OrganisationId}", UriKind.Relative), response);
        }
    }
}
