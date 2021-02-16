﻿using System;
using System.Linq;
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
    [Produces("application/json")]
    [Authorize(AuthenticationSchemes = IdentityServerAuthenticationDefaults.AuthenticationScheme,
        Policy = PolicyName.CanAccessOrganisationUsers)]
    public sealed class UsersController : ControllerBase
    {
        private readonly ICreateBuyerService _createBuyerService;
        private readonly IUsersRepository _usersRepository;

        public UsersController(
            ICreateBuyerService createBuyerService,
            IUsersRepository usersRepository)
        {
            _createBuyerService = createBuyerService ?? throw new ArgumentNullException(nameof(createBuyerService));
            _usersRepository = usersRepository ?? throw new ArgumentNullException(nameof(usersRepository));
        }

        [Route("api/v1/Organisations/{organisationId}/Users")]
        [HttpGet]
        public async Task<ActionResult> GetUsersByOrganisationId(Guid organisationId)
        {
            var organisationUsers = await _usersRepository.FindByOrganisationIdAsync(organisationId);
            var userViewModels = organisationUsers.Select(x => new OrganisationUserModel
            {
                UserId = x.Id,
                FirstName = x.FirstName,
                LastName = x.LastName,
                PhoneNumber = x.PhoneNumber,
                EmailAddress = x.Email,
                IsDisabled = x.Disabled
            });

            return Ok(new GetAllOrganisationUsersModel
            {
                Users = userViewModels
            });
        }

        [Route("api/v1/users/{userId}")]
        [HttpGet]
        public async Task<ActionResult<GetUserModel>> GetUserByIdAsync(string userId)
        {
            var user = await _usersRepository.GetByIdAsync(userId);

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
                PrimaryOrganisationId = user.PrimaryOrganisationId
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

            var result = await _createBuyerService.CreateAsync(new CreateBuyerRequest(
                organisationId,
                createBuyerRequest.FirstName,
                createBuyerRequest.LastName,
                createBuyerRequest.PhoneNumber,
                createBuyerRequest.EmailAddress
                ));

            if (result.IsSuccess)
            {
                response.UserId = result.Value;
                return CreatedAtAction(nameof(GetUserByIdAsync).TrimAsync(), null, new { userId = result.Value }, response);
            }

            response.Errors = result.Errors.Select(x => new ErrorMessageViewModel(x.Id, x.Field));
            return BadRequest(response);
        }

        [Route("api/v1/users/{userid}/enable")]
        [HttpPost]
        [Authorize(Policy = PolicyName.CanManageOrganisationUsers)]
        public async Task<ActionResult> EnableUserAsync(string userId)
        {
            return await ChangingUsersStatusAsync(userId, x => x.MarkAsEnabled());
        }

        [Route("api/v1/users/{userid}/disable")]
        [HttpPost]
        [Authorize(Policy = PolicyName.CanManageOrganisationUsers)]
        public async Task<ActionResult> DisableUserAsync(string userId)
        {
            return await ChangingUsersStatusAsync(userId, x => x.MarkAsDisabled());
        }

        private async Task<ActionResult> ChangingUsersStatusAsync(string userId, Action<ApplicationUser> userAction)
        {
            var user = await _usersRepository.GetByIdAsync(userId);

            if (user is null)
            {
                return NotFound();
            }

            userAction(user);

            await _usersRepository.UpdateAsync(user);
            return NoContent();
        }
    }
}
