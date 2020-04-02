using System;
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
    [Produces("application/json")]
    public sealed class UsersController : Controller
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

        [Route("api/v1/Organisations/{organisationId}/Users")]
        [HttpPost]
        [Authorize(Policy = Policy.CanManageOrganisationUsers)]
        public async Task<ActionResult<CreateBuyerResponseViewModel>> CreateBuyerAsync(Guid organisationId, CreateBuyerRequestViewModel createBuyerRequest)
        {
            if (createBuyerRequest is null)
            {
                throw new ArgumentNullException(nameof(createBuyerRequest));
            }

            var response = new CreateBuyerResponseViewModel();

            var result = await _createBuyerService.CreateAsync(new CreateBuyerRequest(
                organisationId, 
                createBuyerRequest.FirstName,
                createBuyerRequest.LastName,
                createBuyerRequest.PhoneNumber,
                createBuyerRequest.EmailAddress
                ));

            if (!result.IsSuccess)
            {
                response.Errors = result.Errors.Select(x => new ErrorMessageViewModel { Id = x.Id,  Field = x.Field });
            
                return BadRequest(response);
            }
            
            return Ok(response);
        }

        [Route("users/{userId}")]
        [HttpGet]
        public async Task<ActionResult> GetUserById(string userId)
        {
            var user = await _usersRepository.GetUserById(userId);

            if (user is null)
            {
                return NotFound();
            }

            var userViewModel = new GetUserViewModel()
            {
                Name = $"{user.FirstName} {user.LastName}",
                PhoneNumber = user.PhoneNumber,
                EmailAddress =  user.Email,
                Disabled = user.Disabled,
                PrimaryOrganisationId = user.PrimaryOrganisationId
            };

            return Ok(userViewModel);
        }
    }
}
