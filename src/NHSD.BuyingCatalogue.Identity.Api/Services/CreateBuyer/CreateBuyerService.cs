using System;
using System.Threading.Tasks;
using NHSD.BuyingCatalogue.Identity.Api.Models;
using NHSD.BuyingCatalogue.Identity.Api.Repositories;
using NHSD.BuyingCatalogue.Identity.Api.Validators;
using NHSD.BuyingCatalogue.Identity.Common.Results;

namespace NHSD.BuyingCatalogue.Identity.Api.Services.CreateBuyer
{
    public sealed class CreateBuyerService : ICreateBuyerService
    {
        private readonly IApplicationUserValidator _applicationUserValidator;
        private readonly IUsersRepository _usersRepository;
        private readonly IPasswordService _passwordService;
        private readonly IRegistrationService _registrationService;

        public CreateBuyerService(
            IApplicationUserValidator applicationUserValidator,
            IUsersRepository usersRepository,
            IPasswordService passwordService,
            IRegistrationService registrationService)
        {
            _applicationUserValidator = applicationUserValidator ?? throw new ArgumentNullException(nameof(applicationUserValidator));
            _passwordService = passwordService ?? throw new ArgumentNullException(nameof(passwordService));
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
                createBuyerRequest.PrimaryOrganisationId);

            var validationResult = await _applicationUserValidator.ValidateAsync(newApplicationUser);
            if (!validationResult.IsSuccess)
                return Result.Failure<string>(validationResult.Errors);

            await _usersRepository.CreateUserAsync(newApplicationUser);
            var token = await _passwordService.GeneratePasswordResetTokenAsync(newApplicationUser.Email);

            // TODO: discuss exception handling options
            // TODO: consider moving sending e-mail out of process
            // (the current in-process implementation has a significant impact on response time)
            await _registrationService.SendInitialEmailAsync(token);

            return Result.Success(newApplicationUser.Id);
        }
    }
}
