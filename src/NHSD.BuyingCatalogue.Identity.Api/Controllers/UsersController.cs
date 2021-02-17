using System;
using System.Linq;
using System.Net.Mime;
using System.Threading.Tasks;
using IdentityServer4.AccessTokenValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NHSD.BuyingCatalogue.Identity.Api.Models;
using NHSD.BuyingCatalogue.Identity.Api.Repositories;
using NHSD.BuyingCatalogue.Identity.Api.Services.CreateBuyer;
using NHSD.BuyingCatalogue.Identity.Api.ViewModels.Users;
using NHSD.BuyingCatalogue.Identity.Common.Constants;
using NHSD.BuyingCatalogue.Identity.Common.Extensions;
using NHSD.BuyingCatalogue.Identity.Common.ViewModels.Messages;

namespace NHSD.BuyingCatalogue.Identity.Api.Controllers
{
    [ApiController]
    [Produces(MediaTypeNames.Application.Json)]
    [Authorize(AuthenticationSchemes = IdentityServerAuthenticationDefaults.AuthenticationScheme, Policy = PolicyName.CanAccessOrganisationUsers)]
    public sealed class UsersController : ControllerBase
    {
        private readonly ICreateBuyerService createBuyerService;
        private readonly IUsersRepository usersRepository;

        public UsersController(
            ICreateBuyerService createBuyerService,
            IUsersRepository usersRepository)
        {
            this.createBuyerService = createBuyerService ?? throw new ArgumentNullException(nameof(createBuyerService));
            this.usersRepository = usersRepository ?? throw new ArgumentNullException(nameof(usersRepository));
        }

        [Route("api/v1/Organisations/{organisationId}/Users")]
        [HttpGet]
        public async Task<ActionResult> GetUsersByOrganisationId(Guid organisationId)
        {
            var organisationUsers = await usersRepository.FindByOrganisationIdAsync(organisationId);
            var userViewModels = organisationUsers.Select(u => new OrganisationUserModel
            {
                UserId = u.Id,
                FirstName = u.FirstName,
                LastName = u.LastName,
                PhoneNumber = u.PhoneNumber,
                EmailAddress = u.Email,
                IsDisabled = u.Disabled,
            });

            return Ok(new GetAllOrganisationUsersModel { Users = userViewModels });
        }

        [Route("api/v1/users/{userId}")]
        [HttpGet]
        public async Task<ActionResult<GetUserModel>> GetUserByIdAsync(string userId)
        {
            var user = await usersRepository.GetByIdAsync(userId);

            if (user is null)
            {
                return NotFound();
            }

            var getUser = new GetUserModel
            {
                Name = user.DisplayName,
                PhoneNumber = user.PhoneNumber,
                EmailAddress = user.Email,
                Disabled = user.Disabled,
                PrimaryOrganisationId = user.PrimaryOrganisationId,
            };

            return Ok(getUser);
        }

        [Route("api/v1/Organisations/{organisationId}/Users")]
        [HttpPost]
        [Authorize(Policy = PolicyName.CanManageOrganisationUsers)]
        public async Task<ActionResult<CreateBuyerResponseModel>> CreateBuyerAsync(Guid organisationId, CreateBuyerRequestModel createBuyerRequest)
        {
            if (createBuyerRequest is null)
            {
                throw new ArgumentNullException(nameof(createBuyerRequest));
            }

            var response = new CreateBuyerResponseModel();

            var result = await createBuyerService.CreateAsync(new CreateBuyerRequest(
                organisationId,
                createBuyerRequest.FirstName,
                createBuyerRequest.LastName,
                createBuyerRequest.PhoneNumber,
                createBuyerRequest.EmailAddress));

            if (result.IsSuccess)
            {
                response.UserId = result.Value;
                return CreatedAtAction(nameof(GetUserByIdAsync).TrimAsync(), null, new { userId = result.Value }, response);
            }

            response.Errors = result.Errors.Select(d => new ErrorMessageViewModel(d.Id, d.Field));
            return BadRequest(response);
        }

        [Route("api/v1/users/{userId}/enable")]
        [HttpPost]
        [Authorize(Policy = PolicyName.CanManageOrganisationUsers)]
        public async Task<ActionResult> EnableUserAsync(string userId)
        {
            return await ChangingUsersStatusAsync(userId, u => u.MarkAsEnabled());
        }

        [Route("api/v1/users/{userId}/disable")]
        [HttpPost]
        [Authorize(Policy = PolicyName.CanManageOrganisationUsers)]
        public async Task<ActionResult> DisableUserAsync(string userId)
        {
            return await ChangingUsersStatusAsync(userId, u => u.MarkAsDisabled());
        }

        private async Task<ActionResult> ChangingUsersStatusAsync(string userId, Action<ApplicationUser> userAction)
        {
            var user = await usersRepository.GetByIdAsync(userId);

            if (user is null)
            {
                return NotFound();
            }

            userAction(user);

            await usersRepository.UpdateAsync(user);
            return NoContent();
        }
    }
}
