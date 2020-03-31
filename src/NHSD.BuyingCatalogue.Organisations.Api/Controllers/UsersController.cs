using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NHSD.BuyingCatalogue.Organisations.Api.Models;
using NHSD.BuyingCatalogue.Organisations.Api.Repositories;
using NHSD.BuyingCatalogue.Organisations.Api.Services;
using NHSD.BuyingCatalogue.Organisations.Api.ViewModels.Messages;
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
        private readonly ICreateBuyerService _createBuyerService;
        private readonly IUsersRepository _usersRepository;

        public UsersController(
        	ICreateBuyerService createBuyerService, 
        	IUsersRepository usersRepository,
        	IRegistrationService registrationService)
        {
            _createBuyerService = createBuyerService ?? throw new ArgumentNullException(nameof(createBuyerService));
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
        public async Task<ActionResult<CreateBuyerResponseViewModel>> CreateBuyerAsync(Guid organisationId, CreateBuyerRequestViewModel createBuyerRequest)
        {
            if (createBuyerRequest is null)
            {
                throw new ArgumentNullException(nameof(createBuyerRequest));
            }

            var result = await _createBuyerService.CreateAsync(new CreateBuyerRequest(
                organisationId, 
                createBuyerRequest.FirstName,
                createBuyerRequest.LastName,
                createBuyerRequest.PhoneNumber,
                createBuyerRequest.EmailAddress
                ));

            var response = new CreateBuyerResponseViewModel();

            if (!result.IsSuccess)
            {
                response.Errors = result.Errors.Select(x => new ErrorViewModel { Id = x.Id,  Field = x.Field });
            
                return BadRequest(response);
            }
            
            // TODO: discuss exception handling options 
            // TODO: consider moving sending e-mail out of process
            // (the current in-process implementation has a significant impact on response time)
            await _registrationService.SendInitialEmailAsync(newApplicationUser);
            
            return Ok(response);
        }
    }
}
