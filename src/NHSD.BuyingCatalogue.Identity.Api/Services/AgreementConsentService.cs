using System;
using System.Threading.Tasks;
using IdentityServer4.Events;
using IdentityServer4.Models;
using IdentityServer4.Services;
using NHSD.BuyingCatalogue.Identity.Api.Repositories;
using NHSD.BuyingCatalogue.Identity.Common.Results;

namespace NHSD.BuyingCatalogue.Identity.Api.Services
{
    internal sealed class AgreementConsentService : IAgreementConsentService
    {
        private readonly IEventService _eventService;
        private readonly IIdentityServerInteractionService _interaction;
        private readonly IScopeRepository _scopeRepository;

        public AgreementConsentService(
            IEventService eventService,
            IIdentityServerInteractionService interaction,
            IScopeRepository scopeRepository)
        {
            _eventService = eventService ?? throw new ArgumentNullException(nameof(eventService));
            _interaction = interaction ?? throw new ArgumentNullException(nameof(interaction));
            _scopeRepository = scopeRepository ?? throw new ArgumentNullException(nameof(scopeRepository));
        }

        public async Task<bool> IsValidReturnUrl(Uri returnUrl)
        {
            if (returnUrl is null)
                throw new ArgumentNullException(nameof(returnUrl));

            return await _interaction.GetAuthorizationContextAsync(returnUrl.ToString()) != null;
        }

        public async Task<Result<Uri>> GrantConsent(Uri returnUrl, string subjectId)
        {
            if (returnUrl is null)
                throw new ArgumentNullException(nameof(returnUrl));

            var context = await _interaction.GetAuthorizationContextAsync(returnUrl.ToString());
            if (context == null)
                return Result.Failure<Uri>();

            var consent = new ConsentResponse
            {
                RememberConsent = true,
                ScopesConsented = _scopeRepository.Scopes,
            };

            await _interaction.GrantConsentAsync(context, consent);
            await _eventService.RaiseAsync(new ConsentGrantedEvent(
                subjectId,
                context.ClientId,
                context.ScopesRequested,
                _scopeRepository.Scopes,
                true));

            return Result.Success(returnUrl);
        }
    }
}
