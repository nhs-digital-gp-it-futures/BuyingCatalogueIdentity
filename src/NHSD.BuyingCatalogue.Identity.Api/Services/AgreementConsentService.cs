﻿using System;
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
        private readonly IEventService eventService;
        private readonly IIdentityServerInteractionService interaction;
        private readonly IScopeRepository scopeRepository;

        public AgreementConsentService(
            IEventService eventService,
            IIdentityServerInteractionService interaction,
            IScopeRepository scopeRepository)
        {
            this.eventService = eventService ?? throw new ArgumentNullException(nameof(eventService));
            this.interaction = interaction ?? throw new ArgumentNullException(nameof(interaction));
            this.scopeRepository = scopeRepository ?? throw new ArgumentNullException(nameof(scopeRepository));
        }

        public async Task<bool> IsValidReturnUrl(Uri returnUrl)
        {
            if (returnUrl is null)
                throw new ArgumentNullException(nameof(returnUrl));

            return await interaction.GetAuthorizationContextAsync(returnUrl.ToString()) is not null;
        }

        public async Task<Result<Uri>> GrantConsent(Uri returnUrl, string subjectId)
        {
            if (returnUrl is null)
                throw new ArgumentNullException(nameof(returnUrl));

            var context = await interaction.GetAuthorizationContextAsync(returnUrl.ToString());
            if (context is null)
                return Result.Failure<Uri>();

            var consent = new ConsentResponse
            {
                RememberConsent = true,
                ScopesConsented = scopeRepository.Scopes,
            };

            await interaction.GrantConsentAsync(context, consent);
            await eventService.RaiseAsync(new ConsentGrantedEvent(
                subjectId,
                context.ClientId,
                context.ScopesRequested,
                scopeRepository.Scopes,
                true));

            return Result.Success(returnUrl);
        }
    }
}
