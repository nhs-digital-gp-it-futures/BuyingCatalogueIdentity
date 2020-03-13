using Moq;
using NHSD.BuyingCatalogue.Identity.Api.Repositories;
using NHSD.BuyingCatalogue.Identity.Api.Services;

namespace NHSD.BuyingCatalogue.Identity.Api.UnitTests.Builders
{
    internal sealed class ProfileServiceBuilder
    {
        private IUserRepository _userRepository;

        internal ProfileServiceBuilder()
        {
            _userRepository = Mock.Of<IUserRepository>();
        }

        internal static ProfileServiceBuilder Create() => new ProfileServiceBuilder();

        internal ProfileServiceBuilder WithUserRepository(IUserRepository userRepository)
        {
            _userRepository = userRepository;
            return this;
        }

        internal ProfileService Build() => new ProfileService(_userRepository);
    }
}
