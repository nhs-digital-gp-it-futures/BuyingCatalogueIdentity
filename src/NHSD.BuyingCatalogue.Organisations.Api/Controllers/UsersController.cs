using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NHSD.BuyingCatalogue.Organisations.Api.Models;
using NHSD.BuyingCatalogue.Organisations.Api.Repositories;
using NHSD.BuyingCatalogue.Organisations.Api.ViewModels.OrganisationUsers;

namespace NHSD.BuyingCatalogue.Organisations.Api.Controllers
{
    [ApiController]
    [Authorize(Policy = Policy.CanAccessOrganisation)]
    [Route("api/v1/Organisations/{organisationId}/Users")]
    [Produces("application/json")]
    public sealed class UsersController : Controller
    {
        private readonly IUsersRepository _usersRepository;

        public UsersController(IUsersRepository usersRepository)
        {
            _usersRepository = usersRepository;
        }

        private static readonly IList<OrganisationUserViewModel> _users = new List<OrganisationUserViewModel>
        {
            new OrganisationUserViewModel
            {
                UserId = "1234-56789",
                FirstName = "John",
                LastName = "Smith",
                PhoneNumber = "01234567890",
                EmailAddress = "a.b@c.com",
                IsDisabled = false,
                OrganisationId = new Guid("FFE7CB2F-9494-4CC7-A348-420D502956D9")
            },
            new OrganisationUserViewModel
            {
                UserId = "9876-54321",
                FirstName = "Benny",
                LastName = "Hill",
                PhoneNumber = "09876543210",
                EmailAddress = "g.b@z.com",
                IsDisabled = true,
                OrganisationId = new Guid("FFE7CB2F-9494-4CC7-A348-420D502956D9")
            }
        };

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

            _users.Add(userViewModel);

            return Ok();
        }
    }
}
