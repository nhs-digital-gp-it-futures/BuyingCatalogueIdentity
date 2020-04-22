using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NHSD.BuyingCatalogue.Identity.Common.Constants;
using NHSD.BuyingCatalogue.Identity.Common.ViewModels.Messages;
using NHSD.BuyingCatalogue.Organisations.Api.Models;
using NHSD.BuyingCatalogue.Organisations.Api.Repositories;
using NHSD.BuyingCatalogue.Organisations.Api.Services;
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
        private readonly ICreateOrganisationService _createOrganisationService;

        public OrganisationsController(IOrganisationRepository organisationRepository, ICreateOrganisationService createOrganisationService)
        {
            _organisationRepository = organisationRepository ?? throw new ArgumentNullException(nameof(organisationRepository));
            _createOrganisationService = createOrganisationService ?? throw new ArgumentNullException(nameof(createOrganisationService));
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
                throw new ArgumentNullException(nameof(viewModel));

            var result = await _createOrganisationService.CreateAsync(new CreateOrganisationRequest(
                viewModel.OrganisationName,
                viewModel.OdsCode,
                viewModel.PrimaryRoleId,
                viewModel.CatalogueAgreementSigned,
                viewModel.Address is null ? null : new Address
                {
                    Line1 = viewModel.Address.Line1,
                    Line2 = viewModel.Address.Line2,
                    Line3 = viewModel.Address.Line3,
                    Line4 = viewModel.Address.Line4,
                    Town = viewModel.Address.Town,
                    County = viewModel.Address.County,
                    Postcode = viewModel.Address.Postcode,
                    Country = viewModel.Address.Country
                }
            ));

            var response = new CreateOrganisationResponseViewModel();

            if (!result.IsSuccess)
            {
                response.Errors = result.Errors.Select(x => new ErrorMessageViewModel(x.Id, x.Field));
                return BadRequest(response);
            }

            response.OrganisationId = result.Value;
            return Created(new Uri($"/{result.Value}", UriKind.Relative), response);
        }
    }
}
