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
        private readonly IApplicationUserValidator applicationUserValidator;
        private readonly IUsersRepository usersRepository;
        private readonly IPasswordService passwordService;
        private readonly IRegistrationService registrationService;

        public CreateBuyerService(
            IApplicationUserValidator applicationUserValidator,
            IUsersRepository usersRepository,
            IPasswordService passwordService,
            IRegistrationService registrationService)
        {
            this.applicationUserValidator = applicationUserValidator ?? throw new ArgumentNullException(nameof(applicationUserValidator));
            this.passwordService = passwordService ?? throw new ArgumentNullException(nameof(passwordService));
            this.registrationService = registrationService ?? throw new ArgumentNullException(nameof(registrationService));
            this.usersRepository = usersRepository ?? throw new ArgumentNullException(nameof(usersRepository));
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

            var validationResult = await applicationUserValidator.ValidateAsync(newApplicationUser);
            if (!validationResult.IsSuccess)
                return Result.Failure<string>(validationResult.Errors);

            await usersRepository.CreateUserAsync(newApplicationUser);
            var token = await passwordService.GeneratePasswordResetTokenAsync(newApplicationUser.Email);

            // TODO: discuss exception handling options
            // TODO: consider moving sending e-mail out of process
            // (the current in-process implementation has a significant impact on response time)
            await registrationService.SendInitialEmailAsync(token);

            return Result.Success(newApplicationUser.Id);
        }
    }
}
