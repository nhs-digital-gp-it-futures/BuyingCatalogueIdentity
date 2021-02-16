using Moq;
using NHSD.BuyingCatalogue.Identity.Api.Repositories;
using NHSD.BuyingCatalogue.Identity.Api.Services;

namespace NHSD.BuyingCatalogue.Identity.Api.UnitTests.Builders
{
    internal sealed class ProfileServiceBuilder
    {
        private IUsersRepository userRepository;
        private IOrganisationRepository organisationRepository;

        private ProfileServiceBuilder()
        {
            userRepository = Mock.Of<IUsersRepository>();
            organisationRepository = Mock.Of<IOrganisationRepository>();
        }

        internal static ProfileServiceBuilder Create() => new();

        internal ProfileServiceBuilder WithUserRepository(IUsersRepository repository)
        {
            userRepository = repository;
            return this;
        }

        internal ProfileServiceBuilder WithOrganisationRepository(IOrganisationRepository repository)
        {
            organisationRepository = repository;
            return this;
        }

        internal ProfileService Build() => new(userRepository, organisationRepository);
    }
}
