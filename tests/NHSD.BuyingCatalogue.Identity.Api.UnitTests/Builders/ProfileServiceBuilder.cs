using Moq;
using NHSD.BuyingCatalogue.Identity.Api.Repositories;
using NHSD.BuyingCatalogue.Identity.Api.Services;

namespace NHSD.BuyingCatalogue.Identity.Api.UnitTests.Builders
{
    internal sealed class ProfileServiceBuilder
    {
        private IUsersRepository _userRepository;

        internal ProfileServiceBuilder()
        {
            _userRepository = Mock.Of<IUsersRepository>();
        }

        internal static ProfileServiceBuilder Create() => new ProfileServiceBuilder();

        internal ProfileServiceBuilder WithUserRepository(IUsersRepository userRepository)
        {
            _userRepository = userRepository;
            return this;
        }

        internal ProfileService Build() => new ProfileService(_userRepository);
    }
}
