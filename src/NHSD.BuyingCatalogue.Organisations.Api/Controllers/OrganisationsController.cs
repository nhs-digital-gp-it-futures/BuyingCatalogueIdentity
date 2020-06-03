using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mime;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NHSD.BuyingCatalogue.Identity.Common.Constants;
using NHSD.BuyingCatalogue.Identity.Common.Extensions;
using NHSD.BuyingCatalogue.Identity.Common.ViewModels.Messages;
using NHSD.BuyingCatalogue.Organisations.Api.Extensions;
using NHSD.BuyingCatalogue.Organisations.Api.Models;
using NHSD.BuyingCatalogue.Organisations.Api.Repositories;
using NHSD.BuyingCatalogue.Organisations.Api.Services;
using NHSD.BuyingCatalogue.Organisations.Api.ViewModels.Organisations;

namespace NHSD.BuyingCatalogue.Organisations.Api.Controllers
{
    [Authorize(Policy = PolicyName.CanAccessOrganisations)]
    [Route("api/v1/Organisations")]
    [ApiController]
    [Produces(MediaTypeNames.Application.Json)]
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

            return Ok(new GetAllOrganisationsModel
            {
                Organisations = organisationsList?.Select(organisation =>
                    new OrganisationModel
                    {
                        OrganisationId = organisation.OrganisationId,
                        Name = organisation.Name,
                        OdsCode = organisation.OdsCode,
                        PrimaryRoleId = organisation.PrimaryRoleId,
                        CatalogueAgreementSigned = organisation.CatalogueAgreementSigned,
                        Address = organisation.Address is null ? null : new AddressModel
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

            return Ok(new OrganisationModel
            {
                OrganisationId = organisation.OrganisationId,
                Name = organisation.Name,
                OdsCode = organisation.OdsCode,
                PrimaryRoleId = organisation.PrimaryRoleId,
                CatalogueAgreementSigned = organisation.CatalogueAgreementSigned,
                Address = organisation.Address is null ? null : new AddressModel
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
        public async Task<ActionResult> UpdateOrganisationByIdAsync(Guid id, UpdateOrganisationModel model)
        {
            if (model is null)
            {
                throw new ArgumentNullException(nameof(model));
            }
            var organisation = await _organisationRepository.GetByIdAsync(id);

            if (organisation is null)
            {
                return NotFound();
            }

            organisation.CatalogueAgreementSigned = model.CatalogueAgreementSigned;

            await _organisationRepository.UpdateAsync(organisation);

            return NoContent();
        }

        [Authorize(Policy = PolicyName.CanManageOrganisations)]
        [HttpPost]
        public async Task<ActionResult<CreateOrganisationResponseModel>> CreateOrganisationAsync(CreateOrganisationRequestModel model)
        {
            if (model is null)
                throw new ArgumentNullException(nameof(model));

            var address = model.Address;
            var result = await _createOrganisationService.CreateAsync(new CreateOrganisationRequest(
                model.OrganisationName,
                model.OdsCode,
                model.PrimaryRoleId,
                model.CatalogueAgreementSigned,
                address is null ? null : new Address
                {
                    Line1 = address.Line1,
                    Line2 = address.Line2,
                    Line3 = address.Line3,
                    Line4 = address.Line4,
                    Town = address.Town,
                    County = address.County,
                    Postcode = address.Postcode,
                    Country = address.Country
                }
            ));

            var response = new CreateOrganisationResponseModel();

            if (!result.IsSuccess)
            {
                response.Errors = result.Errors.Select(x => new ErrorMessageViewModel(x.Id, x.Field));
                return BadRequest(response);
            }

            response.OrganisationId = result.Value;

            return CreatedAtAction(nameof(GetByIdAsync).TrimAsync(), null, new { id = result.Value }, response);
        }

        [HttpGet]
        [Route("{id}/service-recipients")]
        public async Task<ActionResult> GetServiceRecipientsAsync(Guid id)
        {
            var primaryOrganisationId = User.GetPrimaryOrganisationId();
            if (primaryOrganisationId != id)
            {
                return Forbid();
            }

            var organisation = await _organisationRepository.GetByIdAsync(id);

            if (organisation is null)
            {
                return NotFound();
            }

            var model = new List<ServiceRecipientsModel>
            {
                new ServiceRecipientsModel
                {
                    Name = organisation.Name,
                    OdsCode = organisation.OdsCode
                }
            };

            return Ok(model);
        }
    }
}
