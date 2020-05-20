using System;
using System.Threading.Tasks;
using IdentityServer4.Models;
using IdentityServer4.Stores;
using NHSD.BuyingCatalogue.Identity.Api.Repositories;

namespace NHSD.BuyingCatalogue.Identity.Api.Infrastructure
{
    internal sealed class CatalogueAgreementConsentStore : IUserConsentStore
    {
        private readonly IScopeRepository _scopeRepository;
        private readonly IUsersRepository _userRepository;

        public CatalogueAgreementConsentStore(IScopeRepository scopeRepository, IUsersRepository usersRepository)
        {
            _scopeRepository = scopeRepository ?? throw new ArgumentNullException(nameof(scopeRepository));
            _userRepository = usersRepository ?? throw new ArgumentNullException(nameof(usersRepository));
        }

        public async Task StoreUserConsentAsync(Consent consent)
        {
            if (consent is null)
                throw new ArgumentNullException(nameof(consent));

            var user = await _userRepository.GetByIdAsync(consent.SubjectId);
            user.MarkCatalogueAgreementAsSigned();
            await _userRepository.UpdateAsync(user);
        }

        public async Task<Consent> GetUserConsentAsync(string subjectId, string clientId)
        {
            var user = await _userRepository.GetByIdAsync(subjectId);

            return new Consent
            {
                ClientId = clientId,
                CreationTime = DateTime.UtcNow,
                Expiration = DateTime.MaxValue,
                Scopes = user.CatalogueAgreementSigned
                    ? _scopeRepository.Scopes
                    : null,
                SubjectId = subjectId,
            };
        }

        public Task RemoveUserConsentAsync(string subjectId, string clientId)
        {
            return Task.CompletedTask;
        }
    }
}
