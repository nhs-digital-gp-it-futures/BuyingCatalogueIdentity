using Moq;
using NHSD.BuyingCatalogue.Identity.Api.Repositories;
using NHSD.BuyingCatalogue.Identity.Api.Services;

namespace NHSD.BuyingCatalogue.Identity.Api.UnitTests.Builders
{
    internal sealed class ProfileServiceBuilder
    {
        private IUsersRepository _userRepository;
        private IOrganisationRepository _organisationRepository;

        private ProfileServiceBuilder()
        {
            _userRepository = Mock.Of<IUsersRepository>();
            _organisationRepository = Mock.Of<IOrganisationRepository>();
        }

        internal static ProfileServiceBuilder Create() => new ProfileServiceBuilder();

        internal ProfileServiceBuilder WithUserRepository(IUsersRepository userRepository)
        {
            _userRepository = userRepository;
            return this;
        }

        internal  ProfileServiceBuilder WithOrganisationRepository(IOrganisationRepository organisationRepository)
        {
            _organisationRepository = organisationRepository;
            return this;
        }

        internal ProfileService Build() => new ProfileService(_userRepository, _organisationRepository);
    }
}
