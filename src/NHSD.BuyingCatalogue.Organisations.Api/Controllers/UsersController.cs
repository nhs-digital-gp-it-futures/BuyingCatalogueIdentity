using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NHSD.BuyingCatalogue.Organisations.Api.Models;
using NHSD.BuyingCatalogue.Organisations.Api.Repositories;
using NHSD.BuyingCatalogue.Organisations.Api.Services;
using NHSD.BuyingCatalogue.Organisations.Api.ViewModels.Users;

namespace NHSD.BuyingCatalogue.Organisations.Api.Controllers
{
    [ApiController]
    [Authorize(Policy = Policy.CanAccessOrganisationUsers)]
    [Route("api/v1/Organisations/{organisationId}/Users")]
    [Produces("application/json")]
    public sealed class UsersController : Controller
    {
        private readonly IRegistrationService _registrationService;
        private readonly IUsersRepository _usersRepository;

        public UsersController(
            IUsersRepository usersRepository,
            IRegistrationService registrationService)
        {
            _usersRepository = usersRepository ?? throw new ArgumentNullException(nameof(usersRepository));
            _registrationService = registrationService ?? throw new ArgumentNullException(nameof(registrationService));
        }

        [HttpGet]
        public async Task<ActionResult> GetUsersByOrganisationId(Guid organisationId)
        {
            var organisationUsers = await _usersRepository.GetUsersByOrganisationIdAsync(organisationId);
            var userViewModels = organisationUsers.Select(x => new OrganisationUserViewModel
            {
                UserId = x.Id,
                FirstName = x.FirstName,
                LastName = x.LastName,
                PhoneNumber = x.PhoneNumber,
                EmailAddress = x.Email,
                IsDisabled = x.Disabled
            });

            return Ok(new GetAllOrganisationUsersViewModel
            {
                Users = userViewModels
            });
        }

        [HttpPost]
        [Authorize(Policy = Policy.CanManageOrganisationUsers)]
        public async Task<ActionResult<Guid>> CreateUserAsync(Guid organisationId, CreateUserRequestViewModel viewModel)
        {
            if (viewModel is null)
            {
                throw new ArgumentNullException(nameof(viewModel));
            }

            ApplicationUser newApplicationUser = new ApplicationUser
            {
                Id = Guid.NewGuid().ToString(),
                FirstName = viewModel.FirstName,
                LastName = viewModel.LastName,
                UserName = viewModel.EmailAddress,
                NormalizedUserName = viewModel.EmailAddress?.ToUpperInvariant(),
                PhoneNumber = viewModel.PhoneNumber,
                Email = viewModel.EmailAddress,
                NormalizedEmail = viewModel.EmailAddress?.ToUpperInvariant(),
                PrimaryOrganisationId = organisationId,
                OrganisationFunction = "Buyer",
                CatalogueAgreementSigned = false
            };

            await _usersRepository.CreateUserAsync(newApplicationUser);

            // TODO: discuss exception handling options 
            // TODO: consider moving sending e-mail out of process
            // (the current in-process implementation has a significant impact on response time)
            await _registrationService.SendInitialEmailAsync(newApplicationUser);

            return Ok(newApplicationUser.Id);
        }
    }
}
