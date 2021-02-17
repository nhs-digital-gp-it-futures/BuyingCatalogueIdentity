using System;
using System.Threading.Tasks;
using IdentityServer4.Models;
using IdentityServer4.Stores;
using NHSD.BuyingCatalogue.Identity.Api.Repositories;

namespace NHSD.BuyingCatalogue.Identity.Api.Infrastructure
{
    internal sealed class CatalogueAgreementConsentStore : IUserConsentStore
    {
        private readonly IScopeRepository scopeRepository;
        private readonly IUsersRepository userRepository;

        public CatalogueAgreementConsentStore(IScopeRepository scopeRepository, IUsersRepository usersRepository)
        {
            this.scopeRepository = scopeRepository ?? throw new ArgumentNullException(nameof(scopeRepository));
            userRepository = usersRepository ?? throw new ArgumentNullException(nameof(usersRepository));
        }

        public async Task StoreUserConsentAsync(Consent consent)
        {
            if (consent is null)
                throw new ArgumentNullException(nameof(consent));

            var user = await userRepository.GetByIdAsync(consent.SubjectId);
            user.MarkCatalogueAgreementAsSigned();
            await userRepository.UpdateAsync(user);
        }

        public async Task<Consent> GetUserConsentAsync(string subjectId, string clientId)
        {
            var user = await userRepository.GetByIdAsync(subjectId);

            return new Consent
            {
                ClientId = clientId,
                CreationTime = DateTime.UtcNow,
                Expiration = DateTime.MaxValue,
                Scopes = user.CatalogueAgreementSigned ? scopeRepository.Scopes : null,
                SubjectId = subjectId,
            };
        }

        public Task RemoveUserConsentAsync(string subjectId, string clientId)
        {
            return Task.CompletedTask;
        }
    }
}
