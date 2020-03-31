using System;
using System.Threading.Tasks;
using NHSD.BuyingCatalogue.Organisations.Api.Models;
using NHSD.BuyingCatalogue.Organisations.Api.Repositories;
using NHSD.BuyingCatalogue.Organisations.Api.Validators;

namespace NHSD.BuyingCatalogue.Organisations.Api.Services
{
    public sealed class CreateBuyerService : ICreateBuyerService
    {
        private readonly IApplicationUserValidator _applicationUserValidator;
        private readonly IUsersRepository _usersRepository;

        public CreateBuyerService(
            IApplicationUserValidator applicationUserValidator,
            IUsersRepository usersRepository)
        {
            _applicationUserValidator = applicationUserValidator ?? throw new ArgumentNullException(nameof(applicationUserValidator));
            _usersRepository = usersRepository ?? throw new ArgumentNullException(nameof(usersRepository));
        }

        public async Task<Result> CreateAsync(CreateBuyerRequest createBuyerRequest)
        {
            if (createBuyerRequest is null)
            {
                throw new ArgumentNullException(nameof(createBuyerRequest));
            }

            ApplicationUser newApplicationUser = ApplicationUser.CreateBuyer(
                createBuyerRequest.EmailAddress,
                createBuyerRequest.FirstName,
                createBuyerRequest.LastName,
                createBuyerRequest.PhoneNumber,
                createBuyerRequest.EmailAddress,
                createBuyerRequest.PrimaryOrganisationId
            );

            var result = await _applicationUserValidator.ValidateAsync(newApplicationUser);
            if (result.IsSuccess)
            {
                await _usersRepository.CreateUserAsync(newApplicationUser);
            }

            return result;
        }

    }
}
