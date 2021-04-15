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
    public sealed class OrganisationsController : ControllerBase
    {
        private readonly IOrganisationRepository organisationRepository;
        private readonly ICreateOrganisationService createOrganisationService;
        private readonly IServiceRecipientRepository serviceRecipientRepository;

        public OrganisationsController(
            IOrganisationRepository organisationRepository,
            ICreateOrganisationService createOrganisationService,
            IServiceRecipientRepository serviceRecipientRepository)
        {
            this.organisationRepository = organisationRepository ?? throw new ArgumentNullException(nameof(organisationRepository));
            this.createOrganisationService = createOrganisationService ?? throw new ArgumentNullException(nameof(createOrganisationService));
            this.serviceRecipientRepository = serviceRecipientRepository ?? throw new ArgumentNullException(nameof(serviceRecipientRepository));
        }

        [HttpGet]
        public async Task<ActionResult> GetAllAsync()
        {
            IEnumerable<Organisation> organisationsList = await organisationRepository.ListOrganisationsAsync();

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
                        },
                    }),
            });
        }

        [HttpGet]
        [Route("{organisationId}")]
        public async Task<ActionResult> GetByIdAsync(Guid organisationId)
        {
            var organisation = await organisationRepository.GetByIdAsync(organisationId);

            if (organisation is null)
            {
                return NotFound();
            }

            var addressModel = organisation.Address is null ? null : new AddressModel
            {
                Line1 = organisation.Address.Line1,
                Line2 = organisation.Address.Line2,
                Line3 = organisation.Address.Line3,
                Line4 = organisation.Address.Line4,
                Town = organisation.Address.Town,
                County = organisation.Address.County,
                Postcode = organisation.Address.Postcode,
                Country = organisation.Address.Country,
            };

            return Ok(new OrganisationModel
            {
                OrganisationId = organisation.OrganisationId,
                Name = organisation.Name,
                OdsCode = organisation.OdsCode,
                PrimaryRoleId = organisation.PrimaryRoleId,
                CatalogueAgreementSigned = organisation.CatalogueAgreementSigned,
                Address = addressModel,
            });
        }

        [Authorize(Policy = PolicyName.CanManageOrganisations)]
        [HttpPut]
        [Route("{organisationId}")]
        public async Task<ActionResult> UpdateOrganisationByIdAsync(Guid organisationId, UpdateOrganisationModel model)
        {
            if (model is null)
            {
                throw new ArgumentNullException(nameof(model));
            }

            var organisation = await organisationRepository.GetByIdAsync(organisationId);

            if (organisation is null)
            {
                return NotFound();
            }

            organisation.CatalogueAgreementSigned = model.CatalogueAgreementSigned;

            await organisationRepository.UpdateAsync(organisation);

            return NoContent();
        }

        [Authorize(Policy = PolicyName.CanManageOrganisations)]
        [HttpPost]
        public async Task<ActionResult<CreateOrganisationResponseModel>> CreateOrganisationAsync(CreateOrganisationRequestModel model)
        {
            if (model is null)
                throw new ArgumentNullException(nameof(model));

            var addressModel = model.Address;
            var address = addressModel is null ? null : new Address
            {
                Line1 = addressModel.Line1,
                Line2 = addressModel.Line2,
                Line3 = addressModel.Line3,
                Line4 = addressModel.Line4,
                Town = addressModel.Town,
                County = addressModel.County,
                Postcode = addressModel.Postcode,
                Country = addressModel.Country,
            };

            var result = await createOrganisationService.CreateAsync(new CreateOrganisationRequest(
                model.OrganisationName,
                model.OdsCode,
                model.PrimaryRoleId,
                model.CatalogueAgreementSigned,
                address));

            var response = new CreateOrganisationResponseModel();

            if (!result.IsSuccess)
            {
                response.Errors = result.Errors.Select(d => new ErrorMessageViewModel(d.Id, d.Field));
                return BadRequest(response);
            }

            response.OrganisationId = result.Value;

            return CreatedAtAction(nameof(GetByIdAsync).TrimAsync(), null, new { id = result.Value }, response);
        }

        [HttpGet]
        [Route("{organisationId}/service-recipients")]
        public async Task<ActionResult<IEnumerable<ServiceRecipientsModel>>> GetServiceRecipientsAsync(Guid organisationId)
        {
            var primaryOrganisationId = User.GetPrimaryOrganisationId();
            if (primaryOrganisationId != organisationId)
            {
                return Forbid();
            }

            var organisation = await organisationRepository.GetByIdAsync(organisationId);

            if (organisation is null)
            {
                return NotFound();
            }

            var model = new List<ServiceRecipientsModel>
            {
                new()
                {
                    Name = organisation.Name,
                    OdsCode = organisation.OdsCode,
                },
            };

            var children = await serviceRecipientRepository.GetServiceRecipientsByParentOdsCode(organisation.OdsCode);
            model.AddRange(children.Select(recipient => new ServiceRecipientsModel { Name = recipient.Name, OdsCode = recipient.OrgId }));

            return model.OrderBy(m => m.Name).ToList();
        }

        [HttpGet]
        [Route("{organisationId}/related-organisations")]
        public async Task<ActionResult<IEnumerable<RelatedOrganisationModel>>> GetRelatedOrganisationsAsync(Guid organisationId)
        {
            var organisation = await organisationRepository.GetByIdWithRelatedOrganisationsAsync(organisationId);

            if (organisation is null)
            {
                return NotFound();
            }

            return organisation.RelatedOrganisations
                .Select(ro => new RelatedOrganisationModel { OrganisationId = ro.OrganisationId, Name = ro.Name, OdsCode = ro.OdsCode })
                .ToList();
        }

        [HttpGet]
        [Route("{organisationId}/unrelated-organisations")]
        public async Task<ActionResult<IEnumerable<RelatedOrganisationModel>>> GetUnrelatedOrganisationsAsync(Guid organisationId)
        {
            var organisation = await organisationRepository.GetByIdWithRelatedOrganisationsAsync(organisationId);

            if (organisation is null)
            {
                return NotFound();
            }

            var unrelatedOrganisations = await organisationRepository.GetUnrelatedOrganisations(organisation);

            return unrelatedOrganisations.Select(uo => new RelatedOrganisationModel() { OrganisationId = uo.OrganisationId, Name = uo.Name, OdsCode = uo.OdsCode }).ToList();
        }

        [Authorize(Policy = PolicyName.CanManageOrganisations)]
        [HttpPost]
        [Route("{organisationId}/related-organisations")]
        public async Task<ActionResult> CreateRelatedOrganisationAsync(Guid organisationId, [FromBody] CreateRelatedOrganisationModel model)
        {
            if (model is null)
                throw new ArgumentNullException(nameof(model));

            var organisation = await organisationRepository.GetByIdWithRelatedOrganisationsAsync(organisationId);

            if (organisation is null)
            {
                return NotFound();
            }

            var relatedOrganisation = await organisationRepository.GetByIdAsync(model.RelatedOrganisationId);

            if (relatedOrganisation is null)
            {
                return BadRequest(new ErrorMessageViewModel(FormattableString.Invariant($"The referenced organisation {model.RelatedOrganisationId} cannot be found.")));
            }

            if (organisation.RelatedOrganisations.Any(ro => ro.OrganisationId == model.RelatedOrganisationId))
            {
                return BadRequest(new ErrorMessageViewModel(FormattableString.Invariant($"The referenced organisation {model.RelatedOrganisationId} is already related to {organisation.OrganisationId}.")));
            }

            organisation.RelatedOrganisations.Add(relatedOrganisation);

            await organisationRepository.UpdateAsync(organisation);

            return NoContent();
        }

        [Authorize(Policy = PolicyName.CanAccessOrganisations)]
        [HttpDelete]
        [Route("{organisationId}/related-organisations/{relatedOrganisationId}")]
        public async Task<ActionResult> DeleteRelatedOrganisationAsync(Guid organisationId, Guid relatedOrganisationId)
        {
            var organisation = await organisationRepository.GetByIdWithRelatedOrganisationsAsync(organisationId);

            if (organisation is null)
            {
                return NotFound();
            }

            if (!organisation.RelatedOrganisations.Any(ro => ro.OrganisationId == relatedOrganisationId))
            {
                return BadRequest(new ErrorMessageViewModel(FormattableString.Invariant($"The referenced organisation {organisationId} has no relationship to {relatedOrganisationId}.")));
            }

            var relatedOrganisation = organisation.RelatedOrganisations.Where(ro => ro.OrganisationId == relatedOrganisationId).First();

            organisation.RelatedOrganisations.Remove(relatedOrganisation);

            await organisationRepository.UpdateAsync(organisation);

            return NoContent();
        }
    }
}
