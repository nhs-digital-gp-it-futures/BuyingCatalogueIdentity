using System;
using System.Threading.Tasks;
using NHSD.BuyingCatalogue.Organisations.Api.Models;
using NHSD.BuyingCatalogue.Organisations.Api.Models.Results;
using NHSD.BuyingCatalogue.Organisations.Api.Repositories;
using NHSD.BuyingCatalogue.Organisations.Api.Validators;

namespace NHSD.BuyingCatalogue.Organisations.Api.Services
{
    public sealed class CreateBuyerService : ICreateBuyerService
    {
        private readonly IApplicationUserValidator _applicationUserValidator;
        private readonly IUsersRepository _usersRepository;
        private readonly IRegistrationService _registrationService;

        public CreateBuyerService(
            IApplicationUserValidator applicationUserValidator,
            IUsersRepository usersRepository,
            IRegistrationService registrationService)
        {
            _applicationUserValidator = applicationUserValidator ?? throw new ArgumentNullException(nameof(applicationUserValidator));
            _registrationService = registrationService ?? throw new ArgumentNullException(nameof(registrationService));
            _usersRepository = usersRepository ?? throw new ArgumentNullException(nameof(usersRepository));
        }

        public async Task<Result<string>> CreateAsync(CreateBuyerRequest createBuyerRequest)
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

            var validationResult = await _applicationUserValidator.ValidateAsync(newApplicationUser);
            if (!validationResult.IsSuccess)
                return Result.Failure<string>(validationResult.Errors);
            
            await _usersRepository.CreateUserAsync(newApplicationUser);

            // TODO: discuss exception handling options 
            // TODO: consider moving sending e-mail out of process
            // (the current in-process implementation has a significant impact on response time)
            await _registrationService.SendInitialEmailAsync(newApplicationUser);

            return Result.Success(newApplicationUser.Id);
        }
    }
}
