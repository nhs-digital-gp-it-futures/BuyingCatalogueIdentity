using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using NHSD.BuyingCatalogue.Organisations.Api.Repositories;
using NHSD.BuyingCatalogue.Organisations.Api.ViewModels.OrganisationUsers;

namespace NHSD.BuyingCatalogue.Organisations.Api.Controllers
{
    [ApiController]
    [Route("api/v1/Organisations/{organisationId}/Users")]
    [Produces("application/json")]
    public sealed class UsersController : Controller
    {
        private readonly IUsersRepository _usersRepository;

        public UsersController(IUsersRepository usersRepository)
        {
            _usersRepository = usersRepository ?? throw new ArgumentNullException(nameof(usersRepository));
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
        public ActionResult CreateUser(Guid organisationId, OrganisationUserViewModel userViewModel)
        {
            userViewModel.UserId = Guid.NewGuid().ToString();
            userViewModel.OrganisationId = organisationId;

            //_users.Add(userViewModel);

            return Ok();
        }
    }
}
